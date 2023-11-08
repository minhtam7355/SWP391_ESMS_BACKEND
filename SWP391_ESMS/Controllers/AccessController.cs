using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccessController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IAccessRepository _accessRepo;

        public AccessController(IConfiguration config, IAccessRepository accessRepo)
        {
            _config = config;
            _accessRepo = accessRepo;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            IActionResult response = Unauthorized();
            var user = await _accessRepo.Login(model);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user, model.RememberMe);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        // Helper
        [NonAction]
        private string GenerateJSONWebToken(UserInfo user, bool rememberMe)
        {
            if (user != null)
            {
                var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]!),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(ClaimTypes.Sid, user.UserId.ToString()),
                        new Claim(ClaimTypes.Role, user.Role!)
                    };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                // Define different expiration times based on the 'rememberMe' parameter.
                DateTime? expirationTime = rememberMe
                    ? DateTime.UtcNow.AddDays(Convert.ToDouble(_config["Jwt:ExpiresInRememberMe"]!))  // Longer expiration for "remember me"
                    : DateTime.UtcNow.AddHours(Convert.ToDouble(_config["Jwt:ExpiresIn"]!));   // Shorter expiration for regular sessions

                var token = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Audience"],
                    claims,
                    expires: expirationTime,
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            else
            {
                return "Invalid credentials";
            }
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupModel model)
        {
            IActionResult response = Unauthorized();

            var success = await _accessRepo.Signup(model);

            if (success)
            {
                response = Ok("Signup successful");
            }
            else
            {
                response = BadRequest("Signup failed. Please check your input.");
            }

            return response;

        }
    }
}
