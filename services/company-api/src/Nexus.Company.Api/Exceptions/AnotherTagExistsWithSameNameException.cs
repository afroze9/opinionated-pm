namespace Nexus.CompanyAPI.Exceptions;

public class AnotherTagExistsWithSameNameException : Exception
{
    public AnotherTagExistsWithSameNameException(string name)
        : base($"Another tag exists with the name \"{name}\"")
    {
    }
}