using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using LanguageExt.Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Nexus.PeopleAPI.Abstractions;
using Nexus.PeopleAPI.Configurations;
using Nexus.PeopleAPI.Entities;
using Nexus.PeopleAPI.Exceptions;

namespace Nexus.PeopleAPI.Services;

public class Auth0IdentityService : IIdentityService
{
    private readonly Auth0ManagementOptions _options;
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;
    private readonly ILogger<Auth0IdentityService> _logger;

    public Auth0IdentityService(IOptions<Auth0ManagementOptions> options, HttpClient client, IMemoryCache cache, ILogger<Auth0IdentityService> logger)
    {
        _options = options.Value;
        _client = client;
        _cache = cache;
        _logger = logger;
    }
    
    public async Task<Result<string>> CreateUserAsync(Person person)
    {
        string token = await GetTokenAsync();
        ManagementApiClient client = new (token, new Uri($"https://{_options.Domain}/api/v2"));

        try
        {
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
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error while trying to create user on IDP");
            return new Result<string>(new IdentityServiceException("Error while trying to create user on IDP"));
        }
    }

    public async Task<Result<bool>> DeleteUserAsync(string identityId)
    {
        string token = await GetTokenAsync();
        ManagementApiClient client = new (token, new Uri($"https://{_options.Domain}/api/v2"));

        try
        {
            await client.Users.DeleteAsync(identityId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error while trying to delete user on IDP");
            return new Result<bool>(new IdentityServiceException("Error while trying to delete user on IDP"));
        }
    }

    public async Task<Result<bool>> UpdateAsync(string identityId, string? name, string? email)
    {
        string token = await GetTokenAsync();
        ManagementApiClient client = new (token, new Uri($"https://{_options.Domain}/api/v2"));

        try
        {
            UserUpdateRequest request = new ()
            {
                Connection = _options.Connection,
            };

            if (!string.IsNullOrEmpty(name))
            {
                request.FullName = name;
            }

            if (!string.IsNullOrEmpty(email))
            {
                request.Email = email;
            }
            
            await client.Users.UpdateAsync(identityId, request);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error while trying to update user on IDP");
            return new Result<bool>(new IdentityServiceException("Error while trying to update user on IDP"));
        }
    }

    public async Task<Result<PaginatedList<Person>>> GetUsersRegisteredAfterDate(DateTime date, int pageNum = 0, int pageSize = 50)
    {
        string token = await GetTokenAsync();
        ManagementApiClient client = new (token, new Uri($"https://{_options.Domain}/api/v2"));

        IPagedList<User>? users = await client.Users.GetAllAsync(new GetUsersRequest
        {
            Connection = _options.Connection,
            Query = $"created_at:[{date:yyyy-MM-dd} TO *]",
            SearchEngine = "v2",
        }, new PaginationInfo(pageNum, pageSize, true));

        if (users == null || users.Count == 0)
        {
            return new Result<PaginatedList<Person>>(new Exception("Users not found"));// TODO: Cleanup
        }

        List<Person> mappedUsers = users
            .Select(user => 
                new Person(user.FullName, user.Email)
                {
                    IdentityId = user.UserId,
                })
            .ToList();

        return new Result<PaginatedList<Person>>(new PaginatedList<Person>(mappedUsers, users.Paging.Total, pageNum,
            pageSize));
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