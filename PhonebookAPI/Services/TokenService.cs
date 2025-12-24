using Microsoft.IdentityModel.Tokens;
using PhonebookAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PhonebookAPI.Services
{
  public class TokenService : ITokenService
  {
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
      var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

      // Chiave segreta per firmare il token
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
          _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured")));

      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: _configuration["Jwt:Issuer"],
          audience: _configuration["Jwt:Audience"],
          claims: claims,
          expires: DateTime.UtcNow.AddDays(7), // Token valido 7 giorni
          signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
