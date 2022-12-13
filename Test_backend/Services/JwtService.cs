using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
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



        public dynamic IsAuthenticated(HttpContext context, ControllerBase cb)
        {
            var token = TokenValidation(context);

            if (!token.success) 
            {
                return new
                {
                    success = false,
                    status = cb.StatusCode(StatusCodes.Status401Unauthorized,
                    token.msg)
                };
            }

            return new
            {
                success = true,
                role = token.result.UserRole
            };
        }



        public dynamic IsAuthenticatedPremium(HttpContext context, ControllerBase cb)
        {
            var isAuthenticated = IsAuthenticated(context, cb);

            if (!isAuthenticated.success) return isAuthenticated;

            if (isAuthenticated.role != "premium") 
            {
                return new
                {
                    success = false,
                    status = cb.StatusCode(StatusCodes.Status403Forbidden,
                    "The user has not permission to create a meme.")
                };
            }

            return isAuthenticated;
        }





        public dynamic TokenValidation(HttpContext context)
        {
            var identity = context.User.Identity as ClaimsIdentity;
            return _ValidateToken(identity);

        }


        private static dynamic _ValidateToken(ClaimsIdentity identity)
        {
            try
            {
                if (identity.Claims.Count() == 0)
                {
                    return new
                    {
                        success = false,
                        msg = "Not valid token",
                        result = ""
                    };
                }
                return new
                {
                    success = true,
                    msg = "Validation token successfull",
                    result = new
                    {
                        UserId = identity.Claims.FirstOrDefault(x => x.Type == "Id").Value,
                        Username = identity.Claims.FirstOrDefault(x => x.Type == "Username").Value,
                        UserRole = identity.Claims.FirstOrDefault(x => x.Type == "Role").Value
                    }
                };
            }
            catch (Exception e)
            {
                return new
                {
                    success = false,
                    msg = "Error validation" + e.ToString(),
                    result = ""

                };
            }
        }

    }
}
