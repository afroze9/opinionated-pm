using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Mime;
using AutoMapper;
using FluentValidation;
using LanguageExt.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.PeopleAPI.Abstractions;
using Nexus.PeopleAPI.DTO;
using Nexus.PeopleAPI.Entities;
using Nexus.PeopleAPI.Exceptions;
using Nexus.PeopleAPI.Model;
using Nexus.PeopleAPI.Telemetry;

namespace Nexus.PeopleAPI.Controllers;

[ApiController]
[Route("api/v1")]
public class PeopleController : ControllerBase
{
    private readonly IPeopleService _peopleService;
    private readonly IMapper _mapper;
    private readonly ActivitySource _activitySource;
    private readonly Counter<long> _getAllPeopleCounter;

    public PeopleController(
        IPeopleService peopleService,
        IMapper mapper,
        IPeopleInstrumentation peopleInstrumentation)
    {
        _peopleService = peopleService;
        _mapper = mapper;
        _activitySource = peopleInstrumentation.ActivitySource;
        _getAllPeopleCounter = peopleInstrumentation.GetAllPeopleCounter;
    }
    
    /// <summary>
    ///     Gets list of people.
    /// </summary>
    /// <returns>List of people.</returns>
    [Authorize("read:people")]
    [HttpGet("[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PersonResponseModel>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
    public async Task<ActionResult<List<PersonResponseModel>>> GetAll()
    {
        using Activity? activity = _activitySource.StartActivity("get all people");
        List<PersonDto> people = await _peopleService.GetAllAsync();

        if (people.Count == 0)
        {
            return NotFound();
        }

        List<PersonResponseModel> mappedPeople = _mapper.Map<List<PersonResponseModel>>(people);
        _getAllPeopleCounter.Add(1);
        return Ok(mappedPeople);
    }
    
    /// <summary>
    ///     Gets a person by id.
    /// </summary>
    /// <param name="id">Person id.</param>
    /// <returns>Person by the given id.</returns>
    [Authorize("read:people")]
    [HttpGet("[controller]/{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PersonResponseModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(PersonNotFoundException))]
    public async Task<IActionResult> GetById(int id)
    {
        Result<PersonDto> result =  await _peopleService.GetByIdAsync(id);

        return result.Match<IActionResult>(person => Ok(_mapper.Map<PersonResponseModel>(person)),
            error =>
            {
                if (error is PersonNotFoundException)
                {
                    return NotFound();
                }

                return StatusCode(500, error);
            });
    }

    /// <summary>
    ///     Creates a new person.
    /// </summary>
    /// <param name="model">Person to create.</param>
    /// <returns>Created person.</returns>
    [Authorize("write:people")]
    [HttpPost("[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PersonResponseModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CreatePersonException))]
    public async Task<IActionResult> Create([FromBody] PersonCreateRequestModel model)
    {
        Person person = _mapper.Map<Person>(model);
        Result<Person> createPersonResult = await _peopleService.CreateAsync(person);
        
        return createPersonResult.Match<IActionResult>(
            createdPerson =>
            {
                PersonResponseModel response = _mapper.Map<PersonResponseModel>(createdPerson);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            },
            ex =>
            {
                return ex switch
                {
                    ValidationException => BadRequest(ex),
                    CreatePersonException => StatusCode(500, ex),
                    _ => StatusCode(418),
                };
            });
    }
    
    /// <summary>
    ///     Update person details.
    /// </summary>
    /// <param name="id">Id of the person to update.</param>
    /// <param name="model">Details to update.</param>
    /// <returns>Updated person.</returns>
    [Authorize("update:people")]
    [HttpPut("[controller]/{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PersonResponseModel))]
    public async Task<IActionResult> Update(int id, [FromBody] PersonUpdateRequestModel model)
    {
        Result<Person> result = await _peopleService.UpdateAsync(id, model.Name, model.Email);
        return result.Match<IActionResult>(
            updatedPerson => Ok(_mapper.Map<PersonResponseModel>(updatedPerson)),
            ex =>
            {
                return ex switch
                {
                    PersonNotFoundException => BadRequest(ex),
                    AnotherPersonExistsWithSameEmailException => BadRequest(ex), 
                    ValidationException => BadRequest(ex),
                    _ => StatusCode(500, ex),
                };
            });
    }

    /// <summary>
    ///     Delete a person.
    /// </summary>
    /// <param name="id">Person Id.</param>
    [Authorize("delete:people")]
    [HttpDelete("[controller]/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        Result<bool> result = await _peopleService.DeleteAsync(id);
        return result.Match<IActionResult>(success => NoContent(), ex => StatusCode(500, ex));
    }
}