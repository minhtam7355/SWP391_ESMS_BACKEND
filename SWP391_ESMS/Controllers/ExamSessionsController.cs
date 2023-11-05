using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;
using System.Security.Claims;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamSessionsController : ControllerBase
    {
        private readonly IExamSessionRepository _examRepo;

        public ExamSessionsController(IExamSessionRepository examRepo)
        {
            _examRepo = examRepo;
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff, Teacher")]
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllExamSessions()
        {
            try
            {
                return Ok(await _examRepo.GetAllExamSessionsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff, Teacher, Student")]
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetExamSessionById([FromRoute] Guid id)
        {
            try
            {
                var examSession = await _examRepo.GetExamSessionByIdAsync(id);
                return Ok(examSession);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPost("create")]
        public async Task<IActionResult> AddExamSession([FromBody] ExamSessionModel model)
        {
            try
            {
                var sidClaim = User.FindFirst(ClaimTypes.Sid);

                if (sidClaim != null && Guid.TryParse(sidClaim.Value, out Guid userId)) model.StaffId = userId;
                else return BadRequest("Failed to establish a link with the Staff ID");

                bool result = await _examRepo.AddExamSessionAsync(model);

                if (result)
                {
                    return Ok("Added Successfully");
                }
                else
                {
                    return BadRequest("Failed to add the exam session");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateExamSession([FromBody] ExamSessionModel model)
        {
            try
            {
                bool result = await _examRepo.UpdateExamSessionAsync(model);

                if (result)
                {
                    return Ok("Updated Successfully");
                }
                else
                {
                    return BadRequest("Failed to update the exam session");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteExamSession([FromBody] ExamSessionModel model)
        {
            try
            {
                bool result = await _examRepo.DeleteExamSessionAsync(model);

                if (result)
                {
                    return Ok("Deleted Successfully");
                }
                else
                {
                    return BadRequest("Failed to delete the exam session");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("getbystudent/{studentId}")]
        public async Task<IActionResult> GetExamSessionsByStudent([FromRoute] Guid studentId)
        {
            try
            {
                return Ok(await _examRepo.GetExamSessionsByStudentAsync(studentId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpDelete("removestudent/{examSessionId}/{studentId}")]
        public async Task<IActionResult> RemoveStudentFromExamSession([FromRoute] Guid examSessionId, [FromRoute] Guid studentId)
        {
            try
            {
                bool result = await _examRepo.RemoveStudentFromExamSessionAsync(examSessionId, studentId);

                if (result)
                {
                    return Ok("Student removed from the exam session successfully");
                }
                else
                {
                    return BadRequest("Failed to remove the student from the exam session");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPost("addstudent/{examSessionId}/{studentId}")]
        public async Task<IActionResult> AddStudentToExamSession([FromRoute] Guid examSessionId, [FromRoute] Guid studentId)
        {
            try
            {
                bool result = await _examRepo.AddStudentToExamSessionAsync(examSessionId, studentId);

                if (result)
                {
                    return Ok("Student added to the exam session successfully");
                }
                else
                {
                    return BadRequest("Failed to add the student to the exam session");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
