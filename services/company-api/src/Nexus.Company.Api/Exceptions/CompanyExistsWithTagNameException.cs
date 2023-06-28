namespace Nexus.CompanyAPI.Exceptions;

public class CompanyExistsWithTagNameException : Exception
{
    public CompanyExistsWithTagNameException(string tagName)
    : base($"Companies exist with the tag name \"{tagName}\"")
    {
    }
}