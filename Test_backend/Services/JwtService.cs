using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Test_backend.Models;

namespace Test_backend.Services
{
    public class JwtService
    {
        private IConfiguration _configuration;
        private Jwt jwt;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            jwt = _configuration.GetSection("Jwt").Get<Jwt>();
        }

        public dynamic GenerateJSONWebToken(User user)
        {

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("Id", user.Id),
                new Claim("Username", user.Username),
                new Claim("Role", user.Role),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: credentials);

            return new
            {
                success = true,
                token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

        public dynamic TokenValidation(HttpContext context)
        {
            var identity = context.User.Identity as ClaimsIdentity;
            return Jwt.ValidateToken(identity);

        }
    }
}
