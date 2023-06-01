using AutoMapper;
using PeopleAPI.Abstractions;
using PeopleAPI.Data;
using PeopleAPI.DTO;
using PeopleAPI.Entities;

namespace PeopleAPI.Services;

public class PeopleService : IPeopleService
{
    private readonly IMapper _mapper;
    private readonly UnitOfWork _unitOfWork;
    private readonly ILogger<PeopleService> _logger;

    public PeopleService(
        IMapper mapper,
        UnitOfWork unitOfWork,
        ILogger<PeopleService> logger)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<PersonDto>> GetAllAsync()
    {
        List<Person> people = await _unitOfWork.People.AllAsync();
        return _mapper.Map<List<PersonDto>>(people);
    }

    public async Task<PersonDto> CreateAsync(PersonDto personSummary)
    {
        try
        {
            _unitOfWork.BeginTransaction();
            Person personToCreate = new (personSummary.Name, personSummary.Email);
            _unitOfWork.People.Add(personToCreate);
            _unitOfWork.Commit();

            return _mapper.Map<PersonDto>(personToCreate);
        }
        catch (Exception exception)
        {
            _logger.LogWarning("Error trying to create person {errorMessage}", exception.Message);
            _unitOfWork.Rollback();
            return new PersonDto { Name = "", Email = "" };
        }
    }

    public async Task<PersonDto?> GetByIdAsync(int id)
    {
        Person? person = await _unitOfWork.People.GetByIdAsync(id);
        return _mapper.Map<PersonDto?>(person);
    }

    public async Task<PersonDto?> UpdateNameAsync(int id, string name)
    {
        Person? person = await _unitOfWork.People.GetByIdAsync(id);

        if (person == null)
        {
            return null;
        }
        
        _unitOfWork.BeginTransaction();
        person.UpdateName(name);
        _unitOfWork.Commit();

        return _mapper.Map<PersonDto>(person);
    }

    public async Task DeleteAsync(int id)
    {
        Person? person = await _unitOfWork.People.GetByIdAsync(id);

        if (person == null)
        {
            return;
        }
        
        _unitOfWork.BeginTransaction();
        _unitOfWork.People.Delete(person);
        _unitOfWork.Commit();
    }
}