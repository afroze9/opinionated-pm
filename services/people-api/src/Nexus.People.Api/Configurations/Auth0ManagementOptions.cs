namespace Nexus.PeopleAPI.Configurations;

public class Auth0ManagementOptions
{
    public string Token { get; set; } = string.Empty;

    public string Domain { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string Connection { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;
}