using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamShiftsController : ControllerBase
    {
        private readonly IExamShiftRepository _shiftRepo;

        public ExamShiftsController(IExamShiftRepository shiftRepo)
        {
            _shiftRepo = shiftRepo;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllExamShifts()
        {
            try
            {
                return Ok(await _shiftRepo.GetAllExamShiftsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetExamShiftById([FromRoute] Guid id)
        {
            try
            {
                return Ok(await _shiftRepo.GetExamShiftByIdAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddExamShift([FromBody] ExamShiftModel model)
        {
            try
            {
                bool result = await _shiftRepo.AddExamShiftAsync(model);

                if (result)
                {
                    return Ok("Added Successfully");
                }
                else
                {
                    return BadRequest("Failed to add the exam shift");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateExamShift([FromBody] ExamShiftModel model)
        {
            try
            {
                bool result = await _shiftRepo.UpdateExamShiftAsync(model);

                if (result)
                {
                    return Ok("Updated Successfully");
                }
                else
                {
                    return BadRequest("Failed to update the exam shift");
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteExamShift([FromRoute] Guid id)
        {
            try
            {
                bool result = await _shiftRepo.DeleteExamShiftAsync(id);

                if (result)
                {
                    return Ok("Deleted Successfully");
                }
                else
                {
                    return BadRequest("Failed to delete the exam shift");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
