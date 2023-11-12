using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository _profileRepo;

        public ProfileController(IProfileRepository profileRepo)
        {
            _profileRepo = profileRepo;
        }

        [HttpGet("userprofile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                return Ok(await GetCurrentUserProfileAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("userprofile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserInfo user)
        {
            try
            {
                var currentUser = await GetCurrentUserProfileAsync();
                if (currentUser == null) { return BadRequest("Failed to establish a link with the User"); }
                if (currentUser.Username != user.Username)
                {
                    bool isUsernameAvailable = await _profileRepo.IsUsernameAvailableAsync(user.Username!);

                    if (!isUsernameAvailable)
                    {
                        return BadRequest("Username is already in use");
                    }
                }
                if (currentUser.Email != user.Email)
                {
                    bool isEmailAvailable = await _profileRepo.IsEmailAvailableAsync(user.Email!);

                    if (!isEmailAvailable)
                    {
                        return BadRequest("Email is already in use");
                    }
                }

                bool result = await _profileRepo.UpdateUserProfileAsync(user);

                if (result)
                {
                    return Ok("Updated Successfully");
                }
                else
                {
                    return BadRequest("Failed to update the User");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            try
            {
                var user = await GetCurrentUserProfileAsync();
                if (user == null) { return BadRequest("Failed to establish a link with the User"); }
                if (model.NewPassword != model.ConfirmPassword) return BadRequest("New password and confirm password must be the same");
                if (BC.EnhancedVerify(model.NewPassword, user.PasswordHash)) return BadRequest("New password cannot be the same as the current password");
                if (model.NewPassword!.Contains(user.Username!)) return BadRequest("New password cannot contain the username");
                if (!BC.EnhancedVerify(model.CurrentPassword, user.PasswordHash)) return BadRequest("Incorrect current password");

                bool result = await _profileRepo.ChangePasswordAsync(model, user.UserId, user.Role!);

                if (result)
                {
                    return Ok("Changed Password Successfully");
                }
                else
                {
                    return BadRequest("Failed to change the password");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("uploadprofilepicture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile picture)
        {
            if (picture != null && picture.Length > 0)
            {
                if (picture.Length > 1024 * 1024) // Check if the file size exceeds 1MB
                {
                    // File size is too large, return a warning
                    return BadRequest("The profile picture exceeds the maximum allowed size");
                }

                using (var memoryStream = new MemoryStream())
                {
                    await picture.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();

                    // Convert the image bytes to a base64 string
                    var base64String = Convert.ToBase64String(imageBytes);

                    // Save the base64 string to the database
                    var user = await GetCurrentUserProfileAsync();
                    if (user == null) { return BadRequest("Failed to establish a link with the User"); }
                    bool result = await _profileRepo.SaveProfilePictureAsync(user.UserId, user.Role!, base64String);

                    if (result)
                    {
                        return Ok("Successfully uploaded the profile picture");
                    }
                    else
                    {
                        return BadRequest("Failed to save the profile picture");
                    }
                }
            }

            // No file was provided, return an error
            return BadRequest("No profile picture file was uploaded");
        }


        // Helper method to get the current user's profile
        [NonAction]
        private async Task<UserInfo?> GetCurrentUserProfileAsync()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            if (securityToken != null)
            {
                var sidClaim = securityToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
                var roleClaim = securityToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

                if (sidClaim != null && roleClaim != null && Guid.TryParse(sidClaim.Value, out Guid userId))
                {
                    string userRole = roleClaim.Value;
                    return await _profileRepo.GetUserProfileAsync(userId, userRole);
                }
            }

            throw new Exception("Authentication token is invalid or missing");
        }
    }
}
