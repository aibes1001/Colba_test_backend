using Test_backend.Models;
using Test_backend.Services;
using Microsoft.AspNetCore.Mvc;
using DnsClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace Test_backend.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UsersService _usersService;
        private JwtService _jwtService;

        public UserController(UsersService usersService, JwtService jwtService)
        {
            _usersService = usersService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            try
            {
                var userIsRegistred = await _usersService.GetUserByName(user.Username);

                if (userIsRegistred != null)
                {
                    return this.StatusCode(StatusCodes.Status403Forbidden, "User registred");
                }

                await _usersService.CreateAsync(user);

                return CreatedAtAction(null, new { id = user.Id }, user);
            }
            catch
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }          
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin user)
        {
            try
            {
                var userRegistred = await _usersService.GetUserPass(user);

                if (userRegistred == null)
                {
                    return this.StatusCode(StatusCodes.Status400BadRequest, 
                        "Username or password are not correct");
                }

                var tokenString = _jwtService.GenerateJSONWebToken(userRegistred);
                return Ok(tokenString);
            }
            catch(Exception ex)
            {
                Debug.Print(ex.ToString());
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }
    }
}
