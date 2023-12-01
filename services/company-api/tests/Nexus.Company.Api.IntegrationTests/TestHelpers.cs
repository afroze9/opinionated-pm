using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Nexus.CompanyAPI.IntegrationTests;

public static class TestHelpers
{
    public static string GetToken(this List<Claim> claims, bool includeDefaultClaims = true)
    {
        List<Claim> claimsToAdd = new ();

        foreach (Claim claim in claims)
        {
            claimsToAdd.Add(claim);
        }

        if (includeDefaultClaims)
        {
            List<Claim> defaultClaims = new ()
            {
                new Claim(JwtRegisteredClaimNames.Iss, "test"),
                new Claim(JwtRegisteredClaimNames.Aud, "test"),
            };

            foreach (Claim defaultClaim in defaultClaims)
            {
                claimsToAdd.Add(defaultClaim);
            }
        }

        SymmetricSecurityKey key = new (System.Text.Encoding.UTF8.GetBytes("secret".PadRight((512 / 8), '\0')));

        JwtSecurityTokenHandler tokenHandler = new ();
        SecurityTokenDescriptor tokenDescriptor = new ()
        {
            Subject = new ClaimsIdentity(claimsToAdd),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature),
        };

        JwtSecurityToken? token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static void AddAuthorizationTokenHeader(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }
}