using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffRepository _staffRepo;

        public StaffController(IStaffRepository staffRepo)
        {
            _staffRepo = staffRepo;
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
        public async Task<IActionResult> AddStaff([FromBody] AddStaffModel model)
        {
            try
            {
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

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateStaff([FromRoute] Guid id, [FromBody] UpdateStaffModel model)
        {
            try
            {
                bool result = await _staffRepo.UpdateStaffAsync(id, model);

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
