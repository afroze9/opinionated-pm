namespace Nexus.CompanyAPI.Exceptions;

public class AnotherCompanyExistsWithSameNameException : Exception
{
    public AnotherCompanyExistsWithSameNameException(string name)
        : base($"Another company exists with the name \"{name}\"")
    {
    }
}