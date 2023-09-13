namespace Nexus.PeopleAPI.Model;

[ExcludeFromCodeCoverage]
public class PersonResponseModel
{
    public int Id { get; set; }

    public required string IdentityId { get; set; }
    
    public required string Name { get; set; }

    public required string Email { get; set; }
}