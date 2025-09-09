using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LearningLoop.GerenciamentoAlunosApp.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace LearningLoop.GerenciamentoAlunosApp.Services
{
    public class JwtService : IJwtService
    {
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiresMinutes;

        public JwtService(IConfiguration configuration)
        {
            _secret = configuration["Jwt:Secret"] ?? throw new ArgumentNullException("JWT Secret não configurada");
            _issuer = configuration["Jwt:Issuer"];
            _audience = configuration["Jwt:Audience"];
            _expiresMinutes = int.Parse(configuration["Jwt:ExpiresInMinutes"] ?? "60");
        }

        public string GerarToken(int userId, string email, string perfilNome)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, perfilNome)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
