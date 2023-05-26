namespace ProjectManagement.Configuration;

/// <summary>
///     Represents the settings for accessing a key-value pair in Consul's key-value store.
/// </summary>
public class ConsulKVSettings
{
    /// <summary>
    ///     Gets or sets the URL of the Consul agent.
    /// </summary>
    /// <remarks>
    ///     This is the URL of the Consul agent that the application will communicate with, e.g. "http://localhost:8500".
    /// </remarks>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Consul token to use for authentication.
    /// </summary>
    /// <remarks>
    ///     This is the access token that is required if the Consul agent is configured with ACLs enabled.
    /// </remarks>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the key of the key-value pair to retrieve.
    /// </summary>
    /// <remarks>
    ///     This is the key of the key-value pair to retrieve from Consul's key-value store.
    /// </remarks>
    public string Key { get; set; } = string.Empty;
}