using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamSessionsController : ControllerBase
    {
        private readonly IExamSessionRepository _examRepo;

        public ExamSessionsController(IExamSessionRepository examRepo)
        {
            _examRepo = examRepo;
        }

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

        [HttpPost("create")]
        public async Task<IActionResult> AddExamSession([FromBody] ExamSessionModel model)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                if (securityToken != null)
                {
                    var sidClaim = securityToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
                    if (sidClaim != null && Guid.TryParse(sidClaim.Value, out Guid userId)) model.StaffId = userId;
                    else return BadRequest("Failed to establish a link with the Staff ID");
                }
                else
                {
                    return BadRequest("Failed to establish a link with the Staff ID");
                }

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

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteExamSession([FromRoute] Guid id)
        {
            try
            {
                bool result = await _examRepo.DeleteExamSessionAsync(id);

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

        [HttpPut("removestudent/{examSessionId}/{studentId}")]
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

        [HttpPut("addstudent/{examSessionId}/{studentId}")]
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

        [HttpPut("addteacher/{examSessionId}/{teacherId}")]
        public async Task<IActionResult> AddTeacherToExamSession([FromRoute] Guid examSessionId, [FromRoute] Guid teacherId)
        {
            try
            {
                bool result = await _examRepo.AddTeacherToExamSessionAsync(examSessionId, teacherId);

                if (result)
                {
                    return Ok("Teacher added to the exam session successfully");
                }
                else
                {
                    return BadRequest("Failed to add the teacher to the exam session");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("removeteacher/{examSessionId}")]
        public async Task<IActionResult> RemoveTeacherFromExamSession([FromRoute] Guid examSessionId)
        {
            try
            {
                bool result = await _examRepo.RemoveTeacherFromExamSessionAsync(examSessionId);

                if (result)
                {
                    return Ok("Teacher removed from the exam session successfully");
                }
                else
                {
                    return BadRequest("Failed to remove the teacher from the exam session");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getexamsessionswithoutteacher")]
        public async Task<IActionResult> GetExamSessionsWithoutTeacher()
        {
            try
            {
                return Ok(await _examRepo.GetExamSessionsWithoutTeacherAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getexamsessionsbyteacher/{teacherId}")]
        public async Task<IActionResult> GetExamSessionsByTeacher([FromRoute] Guid teacherId)
        {
            try
            {
                return Ok(await _examRepo.GetExamSessionsByTeacherAsync(teacherId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("exportexcel")]
        public async Task<IActionResult> ExportExcel()
        {
            var examSessionsData = await GetAllExamSessionsData();
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.AddWorksheet(examSessionsData, "Exam Session Records");

                for (int i = 1; i <= examSessionsData.Columns.Count; i++)
                {
                    ws.Column(i).AdjustToContents();
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExamSessions.xlsx");
                }
            }
        }

        // Helper
        [NonAction]
        private async Task<DataTable> GetAllExamSessionsData()
        {
            DataTable dt = new DataTable();
            dt.TableName = "ExamSessionsData";
            dt.Columns.Add("ExamSessionId", typeof(Guid));
            dt.Columns.Add("CourseName", typeof(string));
            dt.Columns.Add("ExamFormat", typeof(string));
            dt.Columns.Add("ExamDate", typeof(string));
            dt.Columns.Add("ShiftName", typeof(string));
            dt.Columns.Add("StartTime", typeof(TimeSpan));
            dt.Columns.Add("EndTime", typeof(TimeSpan));
            dt.Columns.Add("RoomName", typeof(string));
            dt.Columns.Add("StudentsEnrolled", typeof(int));
            dt.Columns.Add("TeacherName", typeof(string));
            dt.Columns.Add("StaffName", typeof(string));
            dt.Columns.Add("IsPassed", typeof(bool));
            dt.Columns.Add("IsPaid", typeof(bool));

            var examSessions = await _examRepo.GetAllExamSessionsAsync();
            if (examSessions.Count > 0)
            {
                foreach (var item in examSessions)
                {
                    string formattedExamDate = String.Format("{0:dd/MM/yyyy}", item.ExamDate);
                    dt.Rows.Add(item.ExamSessionId, item.CourseName, item.ExamFormat, formattedExamDate, item.ShiftName, item.StartTime, item.EndTime, item.RoomName, item.StudentsEnrolled, item.TeacherName, item.StaffName, item.IsPassed, item.IsPaid);
                }
            }

            dt.DefaultView.Sort = "ExamDate DESC, EndTime ASC";
            return dt.DefaultView.ToTable();
        }
    }
}
