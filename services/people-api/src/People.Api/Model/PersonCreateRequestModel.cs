namespace PeopleAPI.Model;

[ExcludeFromCodeCoverage]
public class PersonCreateRequestModel
{
    required public string Name { get; set; }
    
    required public string Email { get; set; }
}