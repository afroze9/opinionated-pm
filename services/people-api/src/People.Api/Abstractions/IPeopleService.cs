using PeopleAPI.DTO;

namespace PeopleAPI.Abstractions;

public interface IPeopleService
{
    Task<List<PersonDto>> GetAllAsync();

    Task<PersonDto> CreateAsync(PersonDto personSummary);

    Task<PersonDto?> GetByIdAsync(int id);

    Task<PersonDto?> UpdateNameAsync(int id, string name);
    
    Task DeleteAsync(int id);
}
