namespace Nexus.PeopleAPI.DTO;

[ExcludeFromCodeCoverage]
public class PersonDto
{
    public int Id { get; set; }
    
    required public string IdentityId { get; set; }

    required public string Name { get; set; }
    
    required public string Email { get; set; }
}