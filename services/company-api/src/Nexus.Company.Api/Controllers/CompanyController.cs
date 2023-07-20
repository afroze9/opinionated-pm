using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Mime;
using AutoMapper;
using FluentValidation;
using LanguageExt.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.CompanyAPI.Abstractions;
using Nexus.CompanyAPI.DTO;
using Nexus.CompanyAPI.Entities;
using Nexus.CompanyAPI.Exceptions;
using Nexus.CompanyAPI.Model;
using Nexus.CompanyAPI.Telemetry;

namespace Nexus.CompanyAPI.Controllers;

[ApiController]
[Route("api/v1")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly IMapper _mapper;
    private readonly ActivitySource _activitySource;
    private readonly Counter<long> _getAllCompaniesCounter;

    public CompanyController(
        ICompanyService companyService, 
        IMapper mapper,
        ICompanyInstrumentation companyInstrumentation)
    {
        _companyService = companyService;
        _mapper = mapper;
        _activitySource = companyInstrumentation.ActivitySource;
        _getAllCompaniesCounter = companyInstrumentation.GetAllCompaniesCounter;
    }

    /// <summary>
    ///     Gets list of companies.
    /// </summary>
    /// <returns>List of companies.</returns>
    [Authorize("read:company")]
    [HttpGet("[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CompanySummaryResponseModel>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
    public async Task<ActionResult<List<CompanySummaryResponseModel>>> GetAll()
    {
        using Activity? activity = _activitySource.StartActivity("get all companies");
        List<CompanySummaryDto> companies = await _companyService.GetAllAsync();

        if (companies.Count == 0)
        {
            return NotFound();
        }

        List<CompanySummaryResponseModel> mappedCompanies = _mapper.Map<List<CompanySummaryResponseModel>>(companies);
        _getAllCompaniesCounter.Add(1);
        return Ok(mappedCompanies);
    }

    /// <summary>
    ///     Gets a company by id.
    /// </summary>
    /// <param name="id">Company id.</param>
    /// <returns>Company by the given id.</returns>
    [Authorize("read:company")]
    [HttpGet("[controller]/{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyResponseModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CompanyNotFoundException))]
    public async Task<IActionResult> GetById(int id)
    {
        Result<CompanyDto> result = await _companyService.GetByIdAsync(id);

        return result.Match<IActionResult>(company => Ok(_mapper.Map<CompanyResponseModel>(company)),
            error =>
            {
                if (error is CompanyNotFoundException)
                {
                    return NotFound();
                }

                return StatusCode(500, error);
            });
    }

    /// <summary>
    ///     Creates a new company.
    /// </summary>
    /// <param name="model">Company to create.</param>
    /// <returns>Created company.</returns>
    [Authorize("write:company")]
    [HttpPost("[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CompanyResponseModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CreateCompanyException))]
    public async Task<IActionResult> Create([FromBody] CompanyRequestModel model)
    {
        Company company = _mapper.Map<Company>(model);
        Result<Company> createCompanyResult = await _companyService.CreateAsync(company);
        
        return createCompanyResult.Match<IActionResult>(
            createdCompany =>
            {
                CompanyResponseModel response = _mapper.Map<CompanyResponseModel>(createdCompany);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            },
            ex =>
            {
                return ex switch
                {
                    ValidationException => BadRequest(ex),
                    AnotherCompanyExistsWithSameNameException => BadRequest(ex),
                    CreateCompanyException => StatusCode(500, ex),
                    _ => StatusCode(418),
                };
            });
    }

    /// <summary>
    ///     Update company details.
    /// </summary>
    /// <param name="id">Id of the company to update.</param>
    /// <param name="model">Details to update.</param>
    /// <returns>Updated company.</returns>
    [Authorize("update:company")]
    [HttpPut("[controller]/{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyResponseModel))]
    public async Task<IActionResult> Update(int id, [FromBody] CompanyUpdateRequestModel model)
    {
        Result<Company> result = await _companyService.UpdateNameAsync(id, model.Name);
        return result.Match<IActionResult>(
            updatedCompany => Ok(_mapper.Map<CompanyResponseModel>(updatedCompany)),
            ex =>
            {
                return ex switch
                {
                    CompanyNotFoundException => BadRequest(ex),
                    AnotherCompanyExistsWithSameNameException => BadRequest(ex),
                    ValidationException => BadRequest(ex),
                    _ => StatusCode(500, ex),
                };
            });
    }

    /// <summary>
    ///     Delete a company.
    /// </summary>
    /// <param name="id">Company Id.</param>
    [Authorize("delete:company")]
    [HttpDelete("[controller]/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _companyService.DeleteAsync(id);
        return NoContent();
    }
}