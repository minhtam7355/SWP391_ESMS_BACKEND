using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamFormatsController : ControllerBase
    {
        private readonly IExamFormatRepository _formatRepo;

        public ExamFormatsController(IExamFormatRepository formatRepo)
        {
            _formatRepo = formatRepo;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllExamFormats()
        {
            try
            {
                return Ok(await _formatRepo.GetAllExamFormatsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetExamFormatById([FromRoute] Guid id)
        {
            try
            {
                var examFormat = await _formatRepo.GetExamFormatByIdAsync(id);
                return Ok(examFormat);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddExamFormat([FromBody] ExamFormatModel model)
        {
            try
            {
                if (await _formatRepo.IsExamFormatUniqueAsync(model.ExamFormatCode!, model.ExamFormatName!))
                {
                    bool result = await _formatRepo.AddExamFormatAsync(model);

                    if (result)
                    {
                        return Ok("Added Successfully");
                    }
                    else
                    {
                        return BadRequest("Failed to add the exam format");
                    }
                }
                else
                {
                    return BadRequest("Exam format code or name already exists. Please choose a unique code and name.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateExamFormat([FromBody] ExamFormatModel model)
        {
            try
            {
                if (await _formatRepo.IsExamFormatUniqueAsync(model.ExamFormatCode!, model.ExamFormatName!))
                {
                    bool result = await _formatRepo.UpdateExamFormatAsync(model);

                    if (result)
                    {
                        return Ok("Updated Successfully");
                    }
                    else
                    {
                        return BadRequest("Failed to update the exam format");
                    }
                }
                else
                {
                    return BadRequest("Exam format code or name already exists. Please choose a unique code and name.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteExamFormat([FromRoute] Guid id)
        {
            try
            {
                bool result = await _formatRepo.DeleteExamFormatAsync(id);

                if (result)
                {
                    return Ok("Deleted Successfully");
                }
                else
                {
                    return BadRequest("Failed to delete the exam format");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
