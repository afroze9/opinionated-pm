namespace Nexus.CompanyAPI.Exceptions;

public class FetchCompanyException : Exception
{
    public const string ExceptionMessage = "Error trying to fetch companies";
    
    public FetchCompanyException(Exception ex)
        : base(ExceptionMessage, ex)
    {
    }
}