using AutoMapper.Internal;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;
using SWP391_ESMS.Services;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IExamSessionRepository _examRepo;
        private readonly IEmailService _emailService;

        public StudentsController(IStudentRepository studentRepo, IExamSessionRepository examRepo, IEmailService emailService)
        {
            _studentRepo = studentRepo;
            _examRepo = examRepo;
            _emailService = emailService;
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
        public async Task<IActionResult> SendMail([FromRoute] Guid examSessionId)
        {
            try
            {
                var students = await _studentRepo.GetStudentsByExamSessionAsync(examSessionId);
                if (students == null || students.Count == 0) return BadRequest("No students found in the exam session to send emails");
                foreach (var student in students)
                {
                    EmailRequest mailrequest = new EmailRequest();
                    mailrequest.ToEmail = student.Email;
                    mailrequest.Subject = "Important Notice: Upcoming Exam Information";
                    mailrequest.Body = await GetHtmlContent(examSessionId);

                    await _emailService.SendEmailAsync(mailrequest);
                }
                // Testing
                //EmailRequest mailrequest = new EmailRequest();
                //mailrequest.ToEmail = "tamtmse173551@fpt.edu.vn";
                //mailrequest.Subject = "Important Notice: Upcoming Exam Information";
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
            string Response = "<div style=\"width:100%;background-color:lightblue;text-align:center;margin:10px\">";
            Response += "<p>Dear Student,</p>";
            Response += "<br><br>";
            Response += "<p>I hope this email finds you well. We are writing to provide you with essential information about your upcoming exam.</p>";
            Response += "<strong>Exam Details:</strong>";
            Response += "<ul>";
            Response += "<li>Course Name: {examSession.CourseName}</li>";
            Response += "<li>Exam Format: {examSession.ExamFormat}</li>";
            Response += "<li>Exam Date and Time: {examSession.ExamDate} {examSession.StartTime}-{examSession.EndTime}</li>";
            Response += "<li>Exam Location: {examSession.RoomName}</li>";
            Response += "</ul>";
            Response += "<p>We wish you the best of luck in your upcoming exam. You've got this!</p>";
            Response += "<br><br>";
            Response += "<p>Best regards,</p>";
            Response += "<p>ESMS</p>";
            Response += "</div>";
            return Response;
        }
    }
}
