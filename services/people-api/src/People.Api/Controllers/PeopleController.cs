using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Mime;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleAPI.Abstractions;
using PeopleAPI.DTO;
using PeopleAPI.Model;
using PeopleAPI.Telemetry;

namespace PeopleAPI.Controllers;

[ApiController]
[Route("api/v1")]
public class PeopleController : ControllerBase
{
    private readonly IValidator<PersonCreateRequestModel> _personCreateRequestModelvalidator;
    private readonly IValidator<PersonUpdateRequestModel> _personUpdateRequestModelvalidator;
    private readonly IPeopleService _peopleService;
    private readonly IMapper _mapper;
    private readonly ActivitySource _activitySource;
    private readonly Counter<long> _getAllPeopleCounter;

    public PeopleController(
        IPeopleService peopleService,
        IMapper mapper,
        IValidator<PersonCreateRequestModel> personCreateRequestModelvalidator,
        IValidator<PersonUpdateRequestModel> personUpdateRequestModelvalidator,
        IPeopleInstrumentation peopleInstrumentation)
    {
        _peopleService = peopleService;
        _mapper = mapper;
        _personCreateRequestModelvalidator = personCreateRequestModelvalidator;
        _personUpdateRequestModelvalidator = personUpdateRequestModelvalidator;
        _activitySource = peopleInstrumentation.ActivitySource;
        _getAllPeopleCounter = peopleInstrumentation.GetAllPeopleCounter;
    }

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

    [Authorize("read:people")]
    [HttpGet("[controller]/{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PersonResponseModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
    public async Task<ActionResult<PersonResponseModel>> GetById(int id)
    {
        PersonDto? person = await _peopleService.GetByIdAsync(id);

        if (person == null)
        {
            return NotFound();
        }

        PersonResponseModel mappedPerson = _mapper.Map<PersonResponseModel>(person);

        return Ok(mappedPerson);
    }

    [Authorize("write:people")]
    [HttpPost("[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PersonResponseModel))]
    public async Task<ActionResult<PersonResponseModel>> Create([FromBody] PersonCreateRequestModel model)
    {
        ValidationResult validationResult = await _personCreateRequestModelvalidator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        }

        PersonDto person = _mapper.Map<PersonDto>(model);

        PersonDto createdPersonSummary = await _peopleService.CreateAsync(person);

        PersonResponseModel response = _mapper.Map<PersonResponseModel>(createdPersonSummary);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [Authorize("update:people")]
    [HttpPut("[controller]/{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PersonResponseModel))]
    public async Task<ActionResult<PersonResponseModel>> Update(int id, [FromBody] PersonUpdateRequestModel model)
    {
        ValidationResult validationResult = await _personUpdateRequestModelvalidator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        }

        PersonDto? updatedPerson = await _peopleService.UpdateNameAsync(id, model.Name);

        if (updatedPerson == null)
        {
            return BadRequest($"Unable to find person with the id {id}");
        }

        PersonResponseModel response = _mapper.Map<PersonResponseModel>(updatedPerson);
        return Ok(response);
    }

    [Authorize("delete:people")]
    [HttpDelete("[controller]/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _peopleService.DeleteAsync(id);
        return NoContent();
    }
}