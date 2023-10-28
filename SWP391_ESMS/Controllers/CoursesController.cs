using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseRepo;

        public CoursesController(ICourseRepository courseRepo)
        {
            _courseRepo = courseRepo;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                return Ok(await _courseRepo.GetAllCoursesAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetCourseById([FromRoute] Guid id)
        {
            try
            {
                return Ok(await _courseRepo.GetCourseByIdAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddCourse([FromBody] AddCourseModel model)
        {
            try
            {
                bool result = await _courseRepo.AddCourseAsync(model);

                if (result) 
                {
                    return Ok("Added Successfully");
                }
                else 
                {
                    return BadRequest("Failed to add the course");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCourse([FromRoute] Guid id, [FromBody] UpdateCourseModel model)
        {
            try
            {
                bool result = await _courseRepo.UpdateCourseAsync(id, model);

                if (result)
                {
                    return Ok("Updated Successfully");
                }
                else
                {
                    return BadRequest("Failed to update the course");
                }

                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute] Guid id)
        {
            try
            {
                bool result = await _courseRepo.DeleteCourseAsync(id);

                if (result)
                {
                    return Ok("Deleted Successfully");
                }
                else
                {
                    return BadRequest("Failed to delete the course");
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
