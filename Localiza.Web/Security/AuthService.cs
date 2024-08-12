using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Localiza.Web.Security;

public class AuthService
{
    private readonly string _key;
    private readonly string _issuer;

    public AuthService(IConfiguration configuration)
    {
        _key = configuration.GetValue<string>("Jwt:Key") ?? "";
        _issuer = configuration.GetValue<string>("Jwt:Issuer") ?? "";
    }

    public string GenerateToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new []
            {
                new Claim(JwtRegisteredClaimNames.Sub,usuario.Id.ToString())
                ,new Claim(ClaimTypes.Name, usuario.Nome)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _issuer,
            Audience = _issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}