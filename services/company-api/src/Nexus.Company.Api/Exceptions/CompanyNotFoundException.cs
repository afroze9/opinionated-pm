namespace Nexus.CompanyAPI.Exceptions;

public class CompanyNotFoundException : Exception
{
    public CompanyNotFoundException(int companyId)
    : base($"Company with the id {companyId} does not exist")
    {
    }
}