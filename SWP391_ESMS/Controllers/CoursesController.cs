using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpGet("getbyperiod/{periodId}")]
        public async Task<IActionResult> GetCoursesByPeriod([FromRoute] Guid periodId)
        {
            try
            {
                return Ok(await _courseRepo.GetCoursesByPeriodAsync(periodId));
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
        public async Task<IActionResult> AddCourse([FromBody] CourseModel model)
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

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCourse([FromBody] CourseModel model)
        {
            try
            {
                bool result = await _courseRepo.UpdateCourseAsync(model);

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

        [HttpPut("removestudent/{courseId}/{studentId}")]
        public async Task<IActionResult> RemoveStudentFromCourse([FromRoute] Guid courseId, [FromRoute] Guid studentId)
        {
            try
            {
                bool result = await _courseRepo.RemoveStudentFromCourseAsync(courseId, studentId);

                if (result)
                {
                    return Ok("Student removed from the course successfully");
                }
                else
                {
                    return BadRequest("Failed to remove the student from the course");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("addstudent/{courseId}/{studentId}")]
        public async Task<IActionResult> AddStudentToCourse([FromRoute] Guid courseId, [FromRoute] Guid studentId)
        {
            try
            {
                bool result = await _courseRepo.AddStudentToCourseAsync(courseId, studentId);

                if (result)
                {
                    return Ok("Student added to the course successfully");
                }
                else
                {
                    return BadRequest("Failed to add the student to the course");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
