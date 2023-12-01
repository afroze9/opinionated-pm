using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Nexus.CompanyAPI.IntegrationTests;

public class TestJwtSecurityTokenHandler : ISecurityTokenValidator
{
    private readonly JwtSecurityTokenHandler _innerHandler = new();
    private readonly TokenValidationParameters _innerValidationParameters = new()
    {
        ValidAudience = "test",
        ValidIssuer = "test",
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("secret".PadRight((512/8), '\0'))),
    };
    
    public bool CanValidateToken => _innerHandler.CanValidateToken;
    
    public int MaximumTokenSizeInBytes
    {
        get => _innerHandler.MaximumTokenSizeInBytes;
        set => _innerHandler.MaximumTokenSizeInBytes = value;
    }
    
    public bool CanReadToken(string securityToken) => true;

    public ClaimsPrincipal ValidateToken(
        string securityToken, 
        TokenValidationParameters validationParameters,
        out SecurityToken validatedToken)
    {
        return _innerHandler.ValidateToken(securityToken, _innerValidationParameters, out validatedToken);
    }
}