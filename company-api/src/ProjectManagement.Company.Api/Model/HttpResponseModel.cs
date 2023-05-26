namespace ProjectManagement.CompanyAPI.Model;

public class HttpResponseModel<T>
{
    private HttpResponseModel(T data, bool isError, string errorMessage)
    {
        Data = data;
        IsError = isError;
        ErrorMessage = errorMessage;
    }

    public T Data { get; }

    public bool IsError { get; }

    public string ErrorMessage { get; }

    public static HttpResponseModel<T> Success(T data)
    {
        return new HttpResponseModel<T>(data, false, string.Empty);
    }

    public static HttpResponseModel<T> Failure(T data, string errorMessage)
    {
        return new HttpResponseModel<T>(data, true, errorMessage);
    }
}