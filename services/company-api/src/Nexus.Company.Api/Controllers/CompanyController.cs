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
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<IActionResult> GetAll()
    {
        using Activity? activity = _activitySource.StartActivity();
        _getAllCompaniesCounter.Add(1);
        
        Result<List<CompanySummaryDto>> result = await _companyService.GetAllAsync();
        return result.Match<IActionResult>(
            companies =>
            {
                List<CompanySummaryResponseModel> mappedCompanies =
                    _mapper.Map<List<CompanySummaryResponseModel>>(companies);

                return Ok(mappedCompanies);
            },
            error =>
            {
                return error switch
                {
                    CompanyNotFoundException => NotFound(),
                    FetchCompanyException => StatusCode(500),
                    _ => StatusCode(500),
                };
            });
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<IActionResult> GetById(int id)
    {
        Result<CompanyDto> result = await _companyService.GetByIdAsync(id);
        return result.Match<IActionResult>(
            company => Ok(_mapper.Map<CompanyResponseModel>(company)),
            error =>
            {
                return error switch
                {
                    CompanyNotFoundException => NotFound(),
                    FetchCompanyException => StatusCode(500),
                    _ => StatusCode(500),
                };
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
            error =>
            {
                return error switch
                {
                    ValidationException => BadRequest(error),
                    AnotherCompanyExistsWithSameNameException => BadRequest(error),
                    CreateCompanyException => StatusCode(500),
                    _ => StatusCode(500),
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
            error =>
            {
                return error switch
                {
                    CompanyNotFoundException => BadRequest(error),
                    AnotherCompanyExistsWithSameNameException => BadRequest(error),
                    ValidationException => BadRequest(error),
                    _ => StatusCode(500),
                };
            });
    }

    /// <summary>
    ///     Delete a company.
    /// </summary>
    /// <param name="id">Company Id.</param>
    [Authorize("delete:company")]
    [HttpDelete("[controller]/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _companyService.DeleteAsync(id);
        return NoContent();
    }
}