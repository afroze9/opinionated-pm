using LanguageExt.Common;
using Nexus.Common.Abstractions;
using Nexus.PeopleAPI.DTO;
using Nexus.PeopleAPI.Entities;

namespace Nexus.PeopleAPI.Abstractions;

public interface IPeopleService : INexusService
{
    Task<List<PersonDto>> GetAllAsync();

    Task<Result<Person>> CreateAsync(Person personSummary);

    Task<Result<PersonDto>> GetByIdAsync(int id);

    Task<Result<Person>> UpdateNameAsync(int id, string name);
    
    Task<Result<bool>> DeleteAsync(int id);

    Task<Result<Person>> UpdateAsync(int id, string? name, string? email);
    
    Task<List<PersonDto>> SearchAsync(string name);
}
