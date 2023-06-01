namespace PeopleAPI.Model;

[ExcludeFromCodeCoverage]
public class PersonUpdateRequestModel
{
    required public int Id { get; set; }
    
    required public string Name { get; set; }
}