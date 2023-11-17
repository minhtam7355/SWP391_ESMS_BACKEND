using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;
using SWP391_ESMS.Services;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IExamSessionRepository _examRepo;
        private readonly IEmailService _emailService;
        private readonly IProfileRepository _profileRepo;

        public StudentsController(IStudentRepository studentRepo, IExamSessionRepository examRepo, IEmailService emailService, IProfileRepository profileRepo)
        {
            _studentRepo = studentRepo;
            _examRepo = examRepo;
            _emailService = emailService;
            _profileRepo = profileRepo;
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
                var currentModel = await _studentRepo.GetStudentByIdAsync(model.StudentId);
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

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStudent([FromRoute] Guid id)
        {
            try
            {
                bool result = await _studentRepo.DeleteStudentAsync(id);

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

        [HttpGet("getbycourse/{courseId}")]
        public async Task<IActionResult> GetStudentsByCourse([FromRoute] Guid courseId)
        {
            try
            {
                return Ok(await _studentRepo.GetStudentsByCourseAsync(courseId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getunenrolledbymajor/{courseId}")]
        public async Task<IActionResult> GetUnenrolledStudentsByMajor([FromRoute] Guid courseId)
        {
            try
            {
                var result = await _studentRepo.GetStudentsNotEnrolledInCourseByMajorAsync(courseId);

                if (result == null)
                {
                    return NotFound("Course or Major not found");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getbysession/{examSessionId}")]
        public async Task<IActionResult> GetStudentsByExamSession([FromRoute] Guid examSessionId)
        {
            try
            {
                return Ok(await _studentRepo.GetStudentsByExamSessionAsync(examSessionId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getunassignedstudents/{courseId}")]
        public async Task<IActionResult> GetUnassignedStudents([FromRoute] Guid courseId)
        {
            try
            {
                return Ok(await _studentRepo.GetUnassignedStudentsAsync(courseId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("sendmails/{examSessionId}")]
        public async Task<IActionResult> SendMails([FromRoute] Guid examSessionId)
        {
            try
            {
                var examSession = await _examRepo.GetExamSessionByIdAsync(examSessionId);
                string formattedExamDate = String.Format("{0:dd/MM/yyyy}", examSession.ExamDate);
                var students = await _studentRepo.GetStudentsByExamSessionAsync(examSessionId);
                if (students == null || students.Count == 0) return BadRequest("No students found in the exam session to send emails");
                foreach (var student in students)
                {
                    EmailRequest mailrequest = new EmailRequest();
                    mailrequest.ToEmail = student.Email;
                    mailrequest.Subject = $"Important Notice: Upcoming Exam Information - {formattedExamDate}";
                    mailrequest.Body = await GetHtmlContent(examSessionId);

                    await _emailService.SendEmailAsync(mailrequest);
                }
                // Testing
                //EmailRequest mailrequest = new EmailRequest();
                //mailrequest.ToEmail = "tamtmse173551@fpt.edu.vn";
                //mailrequest.Subject = $"Important Notice: Upcoming Exam Information - {formattedExamDate}";
                //mailrequest.Body = await GetHtmlContent(examSessionId);
                //await _emailService.SendEmailAsync(mailrequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Helper
        [NonAction]
        private async Task<string> GetHtmlContent(Guid examSessionId)
        {
            var examSession = await _examRepo.GetExamSessionByIdAsync(examSessionId);
            string formattedExamDate = String.Format("{0:dd/MM/yyyy}", examSession.ExamDate);
            string Response = "<div style=\"width:100%;margin:10px;font-family:Arial,sans-serif;font-size:14px;color:black;\">";
            Response += "<p>Dear Student,</p>";
            Response += "<br>";
            Response += "<p>I hope this email finds you well. We are writing to provide you with essential information about your upcoming exam.</p>";
            Response += "<strong>Exam Details:</strong>";
            Response += "<ul>";
            Response += $"<li>Course Name: {examSession.CourseName}</li>";
            Response += $"<li>Exam Format: {examSession.ExamFormatCode} - {examSession.ExamFormatName}</li>";
            Response += $"<li>Exam Date and Time: {formattedExamDate}, {examSession.ShiftName}</li>";
            Response += $"<li>Exam Location: {examSession.RoomName}</li>";
            Response += "</ul>";
            Response += "<p>We wish you the best of luck in your upcoming exam. You've got this!</p>";
            Response += "<br>";
            Response += "<p>Best regards,</p>";
            Response += "<p>ESMS</p>";
            Response += "</div>";
            return Response;
        }
    }
}
