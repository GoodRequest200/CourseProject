using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CourseProject.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CourseProject.Api.Services
{
    public class JwtTokenService : ITokenService
    {
        private readonly JwtOptions _options;
        private readonly SymmetricSecurityKey _signingKey;

        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
            var key = string.IsNullOrWhiteSpace(_options.Key) ? throw new InvalidOperationException("JWT key is not configured") : _options.Key;
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        public string Generate(UserModel user, string roleName)
        {
            var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)
            };

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpiresMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
