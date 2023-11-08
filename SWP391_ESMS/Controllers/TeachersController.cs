using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherRepository _teacherRepo;

        public TeachersController(ITeacherRepository teacherRepo)
        {
            _teacherRepo = teacherRepo;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllTeachers()
        {
            try
            {
                return Ok(await _teacherRepo.GetAllTeachersAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetTeacherById([FromRoute] Guid id)
        {
            try
            {
                return Ok(await _teacherRepo.GetTeacherByIdAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddTeacher([FromBody] TeacherModel model)
        {
            try
            {
                if (model.Password != model.ConfirmPassword) return BadRequest("Password and confirm password must be the same");
                bool result = await _teacherRepo.AddTeacherAsync(model);

                if (result)
                {
                    return Ok("Added Successfully");
                }
                else
                {
                    return BadRequest("Failed to add the Teacher");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateTeacher([FromBody] TeacherModel model)
        {
            try
            {
                bool result = await _teacherRepo.UpdateTeacherAsync(model);

                if (result)
                {
                    return Ok("Updated Successfully");
                }
                else
                {
                    return BadRequest("Failed to update the Teacher");
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTeacher([FromRoute] Guid id)
        {
            try
            {
                bool result = await _teacherRepo.DeleteTeacherAsync(id);

                if (result)
                {
                    return Ok("Deleted Successfully");
                }
                else
                {
                    return BadRequest("Failed to delete the Teacher");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
