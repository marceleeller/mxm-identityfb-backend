using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MxmIdentityfbBackend.Domain.Models;
using MxmIdentityfbBackend.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MxmIdentityfbBackend.Infra.Services;

public class TokenService
{

    private IConfiguration _configuration;
    private AppSettings _applicationSettings;

    public TokenService(IConfiguration configuration, IOptions<AppSettings> applicationSettings)
    {
        _configuration = configuration;
        _applicationSettings = applicationSettings.Value;
    }

    public string GenerateToken(User user)
    {
        Claim[] claims = new Claim[]
        {
            new("username", user.UserName),
            new("email", user.Email),
            new("id", user.Id),
            new("loginTimeStamp", DateTime.UtcNow.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_applicationSettings.Secret));

        var loginCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: loginCredentials,
            claims: claims
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
