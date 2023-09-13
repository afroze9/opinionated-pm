using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt.Common;
using Microsoft.AspNetCore.Authorization;
using Nexus.PeopleAPI.Abstractions;
using Nexus.PeopleAPI.Data;
using Nexus.PeopleAPI.DTO;
using Nexus.PeopleAPI.Entities;
using Nexus.PeopleAPI.Exceptions;

namespace Nexus.PeopleAPI.Services;

public class PeopleService : IPeopleService
{
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;
    private readonly UnitOfWork _unitOfWork;
    private readonly ILogger<PeopleService> _logger;
    private readonly IValidator<Person> _personValidator;

    public PeopleService(
        IIdentityService identityService,
        IMapper mapper,
        UnitOfWork unitOfWork,
        ILogger<PeopleService> logger,
        IValidator<Person> personValidator)
    {
        _identityService = identityService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _personValidator = personValidator;
    }

    [Authorize("read:people")]
    public async Task<List<PersonDto>> GetAllAsync()
    {
        List<Person> people = await _unitOfWork.People.AllAsync();
        return _mapper.Map<List<PersonDto>>(people);
    }
    
    [Authorize("write:people")]
    public async Task<Result<Person>> CreateAsync(Person person)
    {
        // Check if user already exists
        ValidationResult validationResult = await _personValidator.ValidateAsync(person);

        if (!validationResult.IsValid)
        {
            return new Result<Person>(new ValidationException(validationResult.Errors));
        }
        
        if (await _unitOfWork.People.ExistsWithEmailAsync(person.Email))
        {
            return new Result<Person>(new AnotherPersonExistsWithSameEmailException(person.Email));
        }

        // If not, create a user on identity provider
        Result<string> userCreationResult = await _identityService.CreateUserAsync(person);
        return userCreationResult.Match<Result<Person>>(createdUserId =>
            {
                person.IdentityId = createdUserId;
                _unitOfWork.BeginTransaction();
                _unitOfWork.People.Add(person);
                _unitOfWork.Commit();
                return person;
            },
            ex =>
            {
                CreatePersonException personException = new (ex);
                _logger.LogInformation(EventIds.CreatePersonTransactionError, personException, CreatePersonException.ExceptionMessage);
                _unitOfWork.Rollback();
                return new Result<Person>(personException);
            });
    }

    [Authorize("read:people")]
    public async Task<Result<PersonDto>> GetByIdAsync(int id)
    {
        Person? person = await _unitOfWork.People.GetByIdAsync(id);
        
        if (person == null)
        {
            return new Result<PersonDto>(new PersonNotFoundException(id));
        }
        
        return _mapper.Map<PersonDto>(person);
    }

    public async Task<Result<Person>> UpdateNameAsync(int id, string name)
    {
        Person? personToUpdate = await _unitOfWork.People.GetByIdAsync(id);

        if (personToUpdate == null)
        {
            return new Result<Person>(new PersonNotFoundException(id));
        }
        
        personToUpdate.UpdateName(name);
        ValidationResult? validationResult = await _personValidator.ValidateAsync(personToUpdate);
        if (!validationResult.IsValid)
        {
            return new Result<Person>(new ValidationException(validationResult.Errors));
        }
        
        _unitOfWork.BeginTransaction();
        _unitOfWork.Commit();

        return personToUpdate;
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        Person? person = await _unitOfWork.People.GetByIdAsync(id);

        if (person == null)
        {
            return true;
        }
        
        Result<bool> userDeletionResult = await _identityService.DeleteUserAsync(person.IdentityId);

        return userDeletionResult.Match(
            success =>
            {
                _unitOfWork.BeginTransaction();
                _unitOfWork.People.Delete(person);
                _unitOfWork.Commit();
                
                return success;
            }, ex =>
            {
                DeletePersonException personException = new (ex);
                _logger.LogInformation(EventIds.DeletePersonTransactionError, personException, DeletePersonException.ExceptionMessage);
                _unitOfWork.Rollback();
                return new Result<bool>(personException);
            });
    }

    //TODO: Check for both null/empty
    public async Task<Result<Person>> UpdateAsync(int id, string? name, string? email)
    {
        Person? personToUpdate = await _unitOfWork.People.GetByIdAsync(id);

        if (personToUpdate == null)
        {
            return new Result<Person>(new PersonNotFoundException(id));
        }

        if (!string.IsNullOrEmpty(name))
        {
            personToUpdate.UpdateName(name);
        }

        if (!string.IsNullOrEmpty(email))
        {
            if (await _unitOfWork.People.AnyOtherPeopleWithSameEmailAsync(id, email))
            {
                return new Result<Person>(new AnotherPersonExistsWithSameEmailException(email));
            }
            personToUpdate.UpdateEmail(email);
        }
        
        ValidationResult? validationResult = await _personValidator.ValidateAsync(personToUpdate);
        if (!validationResult.IsValid)
        {
            return new Result<Person>(new ValidationException(validationResult.Errors));
        }

        Result<bool> updateResult = await _identityService.UpdateAsync(personToUpdate.IdentityId, name, email);

        return updateResult.Match<Result<Person>>(
            _ =>
            {
                _unitOfWork.BeginTransaction();
                _unitOfWork.Commit();
                return personToUpdate;
            },
            ex =>
            {
                UpdatePersonException personException = new (ex);
                _logger.LogInformation(EventIds.UpdatePersonTransactionError, personException,
                    UpdatePersonException.ExceptionMessage);

                _unitOfWork.Rollback();
                return new Result<Person>(personException);
            });
    }

    public async Task<List<PersonDto>> SearchAsync(string name)
    {
        List<Person> people = await _unitOfWork.People.GetAllByNameAsync(name);
        return _mapper.Map<List<PersonDto>>(people);
    }
}