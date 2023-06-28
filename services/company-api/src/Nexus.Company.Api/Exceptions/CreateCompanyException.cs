namespace Nexus.CompanyAPI.Exceptions;

public class CreateCompanyException : Exception
{
    public const string ExceptionMessage = "Error trying to create a company";
    
    public CreateCompanyException(Exception ex)
        : base(ExceptionMessage, ex)
    {
    }
}
