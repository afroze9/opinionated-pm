using Nexus.PeopleAPI.Entities;

namespace Nexus.PeopleAPI.Abstractions;

public interface IIdentityService
{
    Task<string> CreateUserAsync(Person person);
}