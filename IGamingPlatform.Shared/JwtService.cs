using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IGamingPlatform.Domain;
using Microsoft.IdentityModel.Tokens;

namespace IGamingPlatform.Shared;

public static class JwtService
{
    public static string GenerateToken(User user, string secretKey, int? expires = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username)
            }),
            Expires = DateTime.UtcNow.AddMinutes(expires ?? 60),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static string GetUsernameFromToken(string token, string secretKey)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
        };

        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var usernameClaim = claimsPrincipal.FindFirst(ClaimTypes.Name);
            return usernameClaim?.Value;
        }
        catch (Exception ex)
        {
            // Token validation failed or username claim not found
            // Handle exception or return null as appropriate
            return null;
        }
    }
}