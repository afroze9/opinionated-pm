namespace Nexus.PeopleAPI.Model;

[ExcludeFromCodeCoverage]
public record PersonCreateRequestModel(string Name, string Email, string Password);