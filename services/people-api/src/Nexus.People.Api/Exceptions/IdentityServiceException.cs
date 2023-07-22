namespace Nexus.PeopleAPI.Exceptions;

public class IdentityServiceException : Exception
{
    public IdentityServiceException(string message): base(message)
    {
    }
}