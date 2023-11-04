using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, Testing Admin")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _studentRepo;

        public StudentsController(IStudentRepository studentRepo)
        {
            _studentRepo = studentRepo;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllStudents()
        {
            try
            {
                return Ok(await _studentRepo.GetAllStudentsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetStudentById([FromRoute] Guid id)
        {
            try
            {
                return Ok(await _studentRepo.GetStudentByIdAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddStudent([FromBody] StudentModel model)
        {
            try
            {
                if (model.Password != model.ConfirmPassword) return BadRequest("Password and confirm password must be the same");
                bool result = await _studentRepo.AddStudentAsync(model);

                if (result)
                {
                    return Ok("Added Successfully");
                }
                else
                {
                    return BadRequest("Failed to add the Student");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateStudent([FromBody] StudentModel model)
        {
            try
            {
                bool result = await _studentRepo.UpdateStudentAsync(model);

                if (result)
                {
                    return Ok("Updated Successfully");
                }
                else
                {
                    return BadRequest("Failed to update the Student");
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteStudent([FromBody] StudentModel model)
        {
            try
            {
                bool result = await _studentRepo.DeleteStudentAsync(model);

                if (result)
                {
                    return Ok("Deleted Successfully");
                }
                else
                {
                    return BadRequest("Failed to delete the Student");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("getbysession")]
        public async Task<IActionResult> GetStudentsByExamSession(ExamSessionModel model)
        {
            try
            {
                return Ok(await _studentRepo.GetStudentsByExamSessionAsync(model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
