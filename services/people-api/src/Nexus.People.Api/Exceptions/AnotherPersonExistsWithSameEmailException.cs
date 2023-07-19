namespace Nexus.PeopleAPI.Exceptions;

public class AnotherPersonExistsWithSameEmailException : Exception
{
    public AnotherPersonExistsWithSameEmailException(string email)
        : base($"Another person exists with the email \"{email}\"")
    {
    }
}