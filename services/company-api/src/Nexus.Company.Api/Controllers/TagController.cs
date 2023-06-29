using System.Net.Mime;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Nexus.CompanyAPI.Abstractions;
using Nexus.CompanyAPI.Entities;
using Nexus.CompanyAPI.Exceptions;
using Nexus.CompanyAPI.Model;

namespace Nexus.CompanyAPI.Controllers;

/// <summary>
///     Contains methods for managing tags.
/// </summary>
[ApiController]
[Route("api/v1")]
public class TagController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly IMapper _mapper;
    private readonly IValidator<TagRequestModel> _tagRequestModelValidator;
    private readonly ITagService _tagService;

    public TagController(IMapper mapper, ICompanyService companyService,
        ITagService tagService, IValidator<TagRequestModel> tagRequestModelValidator)
    {
        _mapper = mapper;
        _companyService = companyService;
        _tagService = tagService;
        _tagRequestModelValidator = tagRequestModelValidator;
    }

    /// <summary>
    ///     Gets a list of tags.
    /// </summary>
    /// <returns>List of tags.</returns>
    [HttpGet("[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TagResponseModel>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
    public async Task<ActionResult<List<TagResponseModel>>> GetAll()
    {
        List<Tag> tags = await _tagService.GetAllAsync();
        if (tags.Count == 0)
        {
            return NotFound();
        }

        List<TagResponseModel> response = _mapper.Map<List<TagResponseModel>>(tags);
        return Ok(response);
    }

    /// <summary>
    ///     Gets a tag by id.
    /// </summary>
    /// <param name="id">Tag id.</param>
    /// <returns>Tag by the given id.</returns>
    [HttpGet("[controller]/{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TagRequestModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(TagNotFoundException))]
    public async Task<IActionResult> GetById(int id)
    {
        Result<Tag> result = await _tagService.GetByIdAsync(id);

        return result.Match<IActionResult>(company => Ok(_mapper.Map<TagRequestModel>(company)),
            error =>
            {
                if (error is TagNotFoundException)
                {
                    return NotFound();
                }

                return StatusCode(500, error);
            });
    }
    
    /// <summary>
    ///     Creates a tag.
    /// </summary>
    /// <param name="name">Tag name.</param>
    /// <returns>Created tag.</returns>
    [HttpPost("[controller]")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TagResponseModel))]
    public async Task<IActionResult> Create([FromQuery] string name)
    {
        Result<Tag> result = await _tagService.CreateAsync(name);

        return result.Match<IActionResult>(createdTag =>
            {
                TagResponseModel response = _mapper.Map<TagResponseModel>(createdTag);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            },
            error =>
            {
                return error switch
                {
                    ValidationException => BadRequest(error),
                    AnotherTagExistsWithSameNameException => BadRequest(error),
                    _ => StatusCode(500, error),
                };
            });
    }

    /// <summary>
    ///     Deletes a tag.
    /// </summary>
    /// <param name="name">Tag name.</param>
    [HttpDelete("[controller]")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromQuery] string name)
    {
        Result<bool> result = await _tagService.DeleteAsync(name);

        return result.Match<IActionResult>(
            _ => NoContent(),
            error =>
            {
                return error switch
                {
                    CompanyExistsWithTagNameException => BadRequest(error),
                    _ => StatusCode(500, error),
                };
            });
    }

    /// <summary>
    ///     Add tag to a company
    /// </summary>
    /// <param name="id">Company Id.</param>
    /// <param name="tagName">Tag name.</param>
    /// <returns>Updated company.</returns>
    [HttpPost("company/{id}/tag")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyResponseModel))]
    public async Task<IActionResult> AddCompanyTag(int id, [FromQuery] string tagName)
    {
        ValidationResult validationResult =
            await _tagRequestModelValidator.ValidateAsync(new TagRequestModel { Name = tagName });

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        }

        Result<Company> result = await _companyService.AddTagAsync(id, tagName);

        return result.Match<IActionResult>(updatedCompany => Ok(_mapper.Map<CompanyResponseModel>(updatedCompany)),
            error =>
            {
                if (error is CompanyNotFoundException)
                {
                    return BadRequest(error);
                }

                return StatusCode(500, error);
            });
    }

    /// <summary>
    ///     Delete tag from a company.
    /// </summary>
    /// <param name="id">Company Id.</param>
    /// <param name="tagName">Tag name.</param>
    [HttpDelete("company/{id}/tag")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCompanyTag(int id, [FromQuery] string tagName)
    {
        Result<Company> result = await _companyService.DeleteTagAsync(id, tagName);

        return result.Match<IActionResult>(
            _ => NoContent(),
            error =>
            {
                return error switch
                {
                    CompanyNotFoundException => BadRequest(error),
                    _ => StatusCode(500, error),
                };
            });
    }
}