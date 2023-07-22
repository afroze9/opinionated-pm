namespace Nexus.PeopleAPI.Exceptions;

public class UpdatePersonException : Exception
{
    public const string ExceptionMessage = "Error trying to update a person";
    
    public UpdatePersonException(Exception ex)
        : base(ExceptionMessage, ex)
    {
    }
}