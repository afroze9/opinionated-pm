namespace Nexus.PeopleAPI.Model;

[ExcludeFromCodeCoverage]
public class PersonResponseModel
{
    public int Id { get; set; }

    required public string Name { get; set; }

    required public string Email { get; set; }
}