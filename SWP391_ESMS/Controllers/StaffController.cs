using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly IStaffRepository _staffRepo;
        private readonly IProfileRepository _profileRepo;

        public StaffController(IStaffRepository staffRepo, IProfileRepository profileRepo)
        {
            _staffRepo = staffRepo;
            _profileRepo = profileRepo;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllStaff()
        {
            try
            {
                return Ok(await _staffRepo.GetAllStaffAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetStaffById([FromRoute] Guid id)
        {
            try
            {
                return Ok(await _staffRepo.GetStaffByIdAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddStaff([FromBody] StaffModel model)
        {
            try
            {
                bool isUsernameAvailable = await _profileRepo.IsUsernameAvailableAsync(model.Username!);

                if (!isUsernameAvailable)
                {
                    return BadRequest("Username is already in use");
                }

                bool isEmailAvailable = await _profileRepo.IsEmailAvailableAsync(model.Email!);

                if (!isEmailAvailable)
                {
                    return BadRequest("Email is already in use");
                }

                if (model.Password != model.ConfirmPassword) return BadRequest("Password and confirm password must be the same");
                bool result = await _staffRepo.AddStaffAsync(model);

                if (result)
                {
                    return Ok("Added Successfully");
                }
                else
                {
                    return BadRequest("Failed to add the Staff");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateStaff([FromBody] StaffModel model)
        {
            try
            {
                var currentModel = await _staffRepo.GetStaffByIdAsync(model.StaffId);
                if (currentModel.Username != model.Username)
                {
                    bool isUsernameAvailable = await _profileRepo.IsUsernameAvailableAsync(model.Username!);

                    if (!isUsernameAvailable)
                    {
                        return BadRequest("Username is already in use");
                    }
                }
                if (currentModel.Email != model.Email)
                {
                    bool isEmailAvailable = await _profileRepo.IsEmailAvailableAsync(model.Email!);

                    if (!isEmailAvailable)
                    {
                        return BadRequest("Email is already in use");
                    }
                }

                bool result = await _staffRepo.UpdateStaffAsync(model);

                if (result)
                {
                    return Ok("Updated Successfully");
                }
                else
                {
                    return BadRequest("Failed to update the Staff");
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStaff([FromRoute] Guid id)
        {
            try
            {
                bool result = await _staffRepo.DeleteStaffAsync(id);

                if (result)
                {
                    return Ok("Deleted Successfully");
                }
                else
                {
                    return BadRequest("Failed to delete the Staff");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
