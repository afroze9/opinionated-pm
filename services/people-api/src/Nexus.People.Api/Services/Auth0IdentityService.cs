using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Nexus.PeopleAPI.Abstractions;
using Nexus.PeopleAPI.Configurations;
using Nexus.PeopleAPI.Entities;

namespace Nexus.PeopleAPI.Services;

// TODO: Move to framework perhaps
public class Auth0IdentityService : IIdentityService
{
    private readonly Auth0ManagementOptions _options;
    private readonly HttpClient _client;

    public Auth0IdentityService(IOptions<Auth0ManagementOptions> options, HttpClient client)
    {
        _options = options.Value;
        _client = client;
    }
    
    public async Task<string> CreateUserAsync(Person person)
    {
        string token = await GetTokenAsync();
        ManagementApiClient client = new (token, new Uri($"https://{_options.Domain}/api/v2"));
        
        User result = await client.Users.CreateAsync(new UserCreateRequest()
        {
            Connection = _options.Connection,
            Email = person.Email,
            FullName = person.Name,
            Password = person.Password,
            Blocked = false,
            EmailVerified = false,
        });

        return result.UserId;
    }

    private async Task<string> GetTokenAsync()
    {
        Dictionary<string, string> data = new ()
        {
            { "grant_type", "client_credentials" },
            { "client_id", _options.ClientId },
            { "client_secret", _options.ClientSecret },
            { "audience", _options.Audience },
        };
        
        FormUrlEncodedContent content = new (data);
        HttpResponseMessage res = await _client.PostAsync("/oauth/token", content);

        if (!res.IsSuccessStatusCode)
        {
            return string.Empty;
        }

        TokenResponse? token = await res.Content.ReadFromJsonAsync<TokenResponse>();
        return token != null ? token.AccessToken : string.Empty;
    }
    
    private sealed class TokenResponse
    {
        [JsonProperty("access_token")] internal string AccessToken { get; set; } = string.Empty;
    }
}