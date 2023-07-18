namespace Nexus.PeopleAPI.Exceptions;

public class PersonNotFoundException: Exception
{
    public PersonNotFoundException(int personId)
        : base($"Person with the id {personId} does not exist")
    {
    }
}