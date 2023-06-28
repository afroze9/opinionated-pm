namespace Nexus.CompanyAPI.Exceptions;

public class TagNotFoundException: Exception
{
    public TagNotFoundException(int companyId)
        : base($"Tag with the id {companyId} does not exist")
    {
    }
}