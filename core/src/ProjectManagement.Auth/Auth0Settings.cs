namespace ProjectManagement.Auth;


/// <summary>
///     Represents the settings for authenticating with Auth0.
/// </summary>
public class Auth0Settings
{
    /// <summary>
    ///     Gets or sets the Authority value for the Auth0 authentication.
    /// </summary>
    /// <remarks>
    ///     This is the URL of the Auth0 tenant that the application is registered in, e.g. "https://my-tenant.auth0.com/".
    /// </remarks>
    public string Authority { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Audience value for the Auth0 authentication.
    /// </summary>
    /// <remarks>
    ///     This is the identifier of the API that the application is trying to access, e.g. "https://api.myapp.com/".
    /// </remarks>
    public string Audience { get; set; } = string.Empty;
}
