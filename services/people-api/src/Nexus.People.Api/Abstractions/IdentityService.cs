using LanguageExt.Common;
using Nexus.PeopleAPI.Entities;

namespace Nexus.PeopleAPI.Abstractions;

public interface IIdentityService
{
    Task<Result<string>> CreateUserAsync(Person person);

    Task<Result<bool>> DeleteUserAsync(string identityId);

    Task<Result<bool>> UpdateAsync(string identityId, string? name, string? email);

    Task<Result<PaginatedList<Person>>> GetUsersRegisteredAfterDate(DateTime date, int pageNum = 0, int pageSize = 50);
}