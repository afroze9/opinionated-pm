using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Caching.Memory;
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
    private readonly IMemoryCache _cache;

    public Auth0IdentityService(IOptions<Auth0ManagementOptions> options, HttpClient client, IMemoryCache cache)
    {
        _options = options.Value;
        _client = client;
        _cache = cache;
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
        if (_cache.TryGetValue("auth0_management_token", out string? cachedToken))
        {
            return cachedToken!;
        }
        
        TokenRequest tokenRequest = new()
        {
            ClientId = _options.ClientId,
            ClientSecret = _options.ClientSecret,
            Audience = _options.Audience,
        };
        
        StringContent content = new (JsonConvert.SerializeObject(tokenRequest), null, "application/json");
        HttpRequestMessage request = new (HttpMethod.Post, "/oauth/token");
        request.Content = content;
        
        HttpResponseMessage res = await _client.SendAsync(request);

        if (!res.IsSuccessStatusCode)
        {
            return string.Empty;
        }

        string rawResponse = await res.Content.ReadAsStringAsync();
        TokenResponse? token = JsonConvert.DeserializeObject<TokenResponse>(rawResponse);

        if (token == null)
        {
            return string.Empty;
        }

        _cache.Set("auth0_management_token", token.AccessToken, TimeSpan.FromHours(6));
        return token.AccessToken;
    }
    
    public sealed class TokenResponse
    {
        [JsonProperty("access_token")] public string AccessToken { get; set; } = string.Empty;
    }
    
    public sealed class TokenRequest
    {
        [JsonProperty("grant_type")] public string GrantType { get; set; } = "client_credentials";
        [JsonProperty("client_id")] public required string ClientId { get; set; }
        [JsonProperty("client_secret") ] public required string ClientSecret { get; set; } 
        [JsonProperty("audience") ] public required string Audience { get; set; } 
    }
}