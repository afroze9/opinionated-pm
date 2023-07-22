namespace Nexus.PeopleAPI.Exceptions;

public class DeletePersonException : Exception
{
    public const string ExceptionMessage = "Error trying to delete a person";
    
    public DeletePersonException(Exception ex)
        : base(ExceptionMessage, ex)
    {
    }
}