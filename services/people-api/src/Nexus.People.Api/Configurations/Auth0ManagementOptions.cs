namespace Nexus.PeopleAPI.Configurations;

public class Auth0ManagementOptions
{
    public string Token { get; set; }

    public string Domain { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Connection { get; set; }

    public string Audience { get; set; }
}