using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MajorsController : ControllerBase
    {
        private readonly IMajorRepository _majorRepo;

        public MajorsController(IMajorRepository majorRepo)
        {
            _majorRepo = majorRepo;
        }


        [HttpGet("getall")]
        public async Task<IActionResult> GetAllMajors()
        {
            try
            {
                return Ok(await _majorRepo.GetAllMajorsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetMajorById([FromRoute] Guid id)
        {
            try
            {
                return Ok(await _majorRepo.GetMajorByIdAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddMajor([FromBody] MajorModel model)
        {
            try
            {
                bool result = await _majorRepo.AddMajorAsync(model);

                if (result)
                {
                    return Ok("Added Successfully");
                }
                else
                {
                    return BadRequest("Failed to add the major");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateMajor([FromBody] MajorModel model)
        {
            try
            {
                bool result = await _majorRepo.UpdateMajorAsync(model);

                if (result)
                {
                    return Ok("Updated Successfully");
                }
                else
                {
                    return BadRequest("Failed to update the major");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMajor([FromRoute] Guid id)
        {
            try
            {
                bool result = await _majorRepo.DeleteMajorAsync(id);

                if (result)
                {
                    return Ok("Deleted Successfully");
                }
                else
                {
                    return BadRequest("Failed to delete the major");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
