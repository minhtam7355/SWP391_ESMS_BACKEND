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
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly IExamSessionRepository _examRepo;
        private readonly IEmailService _emailService;
        private readonly IProfileRepository _profileRepo;

        public TeachersController(ITeacherRepository teacherRepo, IExamSessionRepository examRepo, IEmailService emailService, IProfileRepository profileRepo)
        {
            _teacherRepo = teacherRepo;
            _examRepo = examRepo;
            _emailService = emailService;
            _profileRepo = profileRepo;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllTeachers()
        {
            try
            {
                var teachers = await _teacherRepo.GetAllTeachersAsync();
                foreach (var teacher in teachers)
                {
                    teacher.CurrentWage = await _teacherRepo.CalculateCurrentWagesAsync(teacher.TeacherId);
                    teacher.TotalEarnings = await _teacherRepo.CalculateTotalEarningsAsync(teacher.TeacherId);
                }
                return Ok(teachers);
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
                var teacher = await _teacherRepo.GetTeacherByIdAsync(id);
                teacher.CurrentWage = await _teacherRepo.CalculateCurrentWagesAsync(id);
                teacher.TotalEarnings = await _teacherRepo.CalculateTotalEarningsAsync(id);
                return Ok(teacher);
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
                var currentModel = await _teacherRepo.GetTeacherByIdAsync(model.TeacherId);
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

        [HttpPost("sendmail/{examSessionId}")]
        public async Task<IActionResult> SendMail([FromRoute] Guid examSessionId)
        {
            try
            {
                var examSession = await _examRepo.GetExamSessionByIdAsync(examSessionId);
                string formattedExamDate = String.Format("{0:dd/MM/yyyy}", examSession.ExamDate);
                var teacher = await _teacherRepo.GetTeacherByExamSessionAsync(examSessionId);
                if (teacher == null) return BadRequest("No teacher found in the exam session to send email");

                EmailRequest mailrequest = new EmailRequest();
                mailrequest.ToEmail = teacher.Email;
                mailrequest.Subject = $"Exam Proctoring Assignment - {formattedExamDate}";
                mailrequest.Body = await GetHtmlContent(examSessionId);

                await _emailService.SendEmailAsync(mailrequest);

                // Testing
                //EmailRequest mailrequest = new EmailRequest();
                //mailrequest.ToEmail = "tamtmse173551@fpt.edu.vn";
                //mailrequest.Subject = $"Exam Proctoring Assignment - {formattedExamDate}";
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
            Response += "<p>Dear Teacher,</p>";
            Response += "<br>";
            Response += $"<p>We would like to inform you that you have been assigned to proctor the upcoming exam on {formattedExamDate}. Your role as a proctor is crucial for a smooth and fair examination process.</p>";
            Response += "<strong>Exam Details:</strong>";
            Response += "<ul>";
            Response += $"<li>Course Name: {examSession.CourseName}</li>";
            Response += $"<li>Exam Format: {examSession.ExamFormatCode} - {examSession.ExamFormatName}</li>";
            Response += $"<li>Exam Date and Time: {formattedExamDate}, {examSession.ShiftName}</li>";
            Response += $"<li>Exam Location: {examSession.RoomName}</li>";
            Response += "</ul>";
            Response += "<p>In case of any issues or questions during the exam, please contact examschedulemanagementsystem@gmail.com. Your support in ensuring the integrity of the exam is greatly appreciated.</p>";
            Response += "<p>Thank you for your dedication to the examination process.</p>";
            Response += "<br>";
            Response += "<p>Best regards,</p>";
            Response += "<p>ESMS</p>";
            Response += "</div>";
            return Response;
        }
    }
}
