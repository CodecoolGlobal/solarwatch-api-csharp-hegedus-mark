using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SolarWatch.Controllers;

namespace SolarWatch.Services.Authentication;

public class TokenService : ITokenService
{
    private readonly IOptions<JwtSettings> _jwtSettings;

    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    private const int ExpirationMinutes = 30;

    public string CreateToken(IdentityUser user)
    {
        var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);

        var token = CreateJwtToken(
            CreateClaims(user),
            CreateSigningCredentials(),
            expiration
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(token);
    }

    private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials signingCredentials,
        DateTime expiration) => new(_jwtSettings.Value.Issuer, _jwtSettings.Value.Audience,
        claims,
        expires: expiration,
        signingCredentials: signingCredentials);

    private List<Claim> CreateClaims(IdentityUser user)
    {
        try
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, "TokenForTheApiWithAuth"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    EpochTime.GetIntDate(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture),
                    ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };
            return claims;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    private SigningCredentials CreateSigningCredentials()
    {
        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey)
            ), SecurityAlgorithms.HmacSha256
        );
    }
}