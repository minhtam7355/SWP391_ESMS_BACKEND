﻿using ClosedXML.Excel;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamSessionsController : ControllerBase
    {
        private readonly IExamSessionRepository _examRepo;
        private readonly IExamPeriodRepository _periodRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IExamShiftRepository _shiftRepo;
        private readonly IConfigurationSettingRepository _settingRepo;

        public ExamSessionsController(IExamSessionRepository examRepo, IExamPeriodRepository periodRepo, ICourseRepository courseRepo, IExamShiftRepository shiftRepo, IConfigurationSettingRepository settingRepo)
        {
            _examRepo = examRepo;
            _periodRepo = periodRepo;
            _courseRepo = courseRepo;
            _shiftRepo = shiftRepo;
            _settingRepo = settingRepo;
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

        [HttpGet("getbyperiod/{periodId}")]
        public async Task<IActionResult> GetExamSessionsByPeriod([FromRoute] Guid periodId)
        {
            try
            {
                return Ok(await _examRepo.GetExamSessionsByPeriodAsync(periodId));
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
                    else return BadRequest("Unable to establish a link with the Staff ID");
                }
                else
                {
                    return BadRequest("Authentication token is invalid or missing");
                }

                var examPeriod = await _periodRepo.GetExamPeriodByIdAsync(model.ExamPeriodId ?? Guid.Empty);
                DateTime minAllowedDate = await GetMinAllowedSchedulingDateAsync();
                if (minAllowedDate > examPeriod!.EndDate)
                {
                    return BadRequest("The scheduling period for exams has ended");
                }
                if (model.ExamDate < minAllowedDate || model.ExamDate > examPeriod!.EndDate)
                {
                    return BadRequest($"The exam date '{String.Format("{0:dd/MM/yyyy}", model.ExamDate)}' is not allowed. Exams can be scheduled starting from '{minAllowedDate.ToString("dd/MM/yyyy")}' and end at '{String.Format("{0:dd/MM/yyyy}", examPeriod.EndDate)}'");
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
                var examSession = await _examRepo.GetExamSessionByIdAsync(model.ExamSessionId);
                var examPeriod = await _periodRepo.GetExamPeriodByIdAsync(examSession.ExamPeriodId ?? Guid.Empty);
                if (model.ExamDate != examSession.ExamDate)
                {
                    if (model.ExamDate <= DateTime.Today)
                    {
                        return BadRequest("Failed to update the exam session. Exam sessions cannot be updated to today or in the past. Please provide a valid future date");
                    }

                    if (model.ExamDate < examPeriod.StartDate || model.ExamDate > examPeriod.EndDate)
                    {
                        return BadRequest("Failed to update the exam session. The new exam date is outside the period of the associated exam period");
                    }

                }
                if (model.ExamDate != examSession.ExamDate || model.ShiftId != examSession.ShiftId || model.RoomId != examSession.RoomId)
                {
                    bool isAvailable = await _examRepo.IsExamSessionAvailableAsync(model.ExamDate, model.ShiftId, model.RoomId);
                    if (!isAvailable)
                    {
                        return BadRequest("Failed to update the exam session. The specified exam session conflicts with an existing session");
                    }
                }

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

        [HttpGet("getexamsessionswithoutteacherbyperiod/{periodId}")]
        public async Task<IActionResult> GetExamSessionsWithoutTeacherByPeriod([FromRoute] Guid periodId)
        {
            try
            {
                Guid teacherId;
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                if (securityToken != null)
                {
                    var sidClaim = securityToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
                    if (sidClaim != null && Guid.TryParse(sidClaim.Value, out Guid userId)) teacherId = userId;
                    else return BadRequest("Unable to establish a link with the Staff ID");
                }
                else
                {
                    return BadRequest("Authentication token is invalid or missing");
                }

                return Ok(await _examRepo.GetExamSessionsWithoutTeacherByPeriodAsync(periodId, teacherId));
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

        [HttpGet("exportexcel/{periodId}")]
        public async Task<IActionResult> ExportExcel([FromRoute] Guid periodId)
        {
            var examSessionsData = await GetAllExamSessionsData(periodId);
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

        [HttpPost("uploadexcel/{periodId}")]
        public async Task<IActionResult> UploadExcel([FromRoute] Guid periodId, IFormFile file)
        {
            // Initialize user ID, message, and minimum allowed date
            Guid userId = Guid.Empty;
            string msg = "";
            DateTime minAllowedDate = await GetMinAllowedSchedulingDateAsync();
            var examPeriod = await _periodRepo.GetExamPeriodByIdAsync(periodId);
            if (minAllowedDate > examPeriod!.EndDate)
            {
                return BadRequest("The scheduling period for exams has ended");
            }
            var courses = await _courseRepo.GetCoursesByPeriodAsync(periodId);
            List<Guid> coursesId = new List<Guid>();
            foreach (var course in courses)
            {
                coursesId.Add(course.CourseId);
            }

            // Extract the user's authentication token
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            if (securityToken != null)
            {
                var sidClaim = securityToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
                if (sidClaim == null || !Guid.TryParse(sidClaim.Value, out userId)) return BadRequest("Unable to establish a link with the Staff ID");
            }
            else
            {
                return BadRequest("Authentication token is invalid or missing");
            }

            // Ensure the file format is correct
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // List to store validated exam sessions
            var examSessions = new List<ExamSessionModel>();

            if (file != null && file.Length > 0)
            {
                //// Define the upload folder and file path
                //var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\";

                //if (!Directory.Exists(uploadsFolder))
                //{
                //    Directory.CreateDirectory(uploadsFolder);
                //}

                //var filePath = Path.Combine(uploadsFolder, file.FileName);

                //// Save the uploaded file
                //using (var stream = new FileStream(filePath, FileMode.Create))
                //{
                //    await file.CopyToAsync(stream);
                //}

                // Validate the contents of the Excel file
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            bool isHeaderSkipped = false;

                            while (reader.Read())
                            {
                                // Check if there is any data in the current row
                                if (reader.IsDBNull(0) && reader.IsDBNull(1) && reader.IsDBNull(2))
                                {
                                    // Break the loop if there is no data in the current row
                                    break;
                                }

                                // Check if any of the cells is null
                                if (reader.IsDBNull(0) || reader.IsDBNull(1) || reader.IsDBNull(2))
                                {
                                    // Return BadRequest if any of the cells is null
                                    return BadRequest("Invalid data format. Each row should contain data in all three columns");
                                }

                                if (!isHeaderSkipped)
                                {
                                    // Skip the header row
                                    isHeaderSkipped = true;
                                    continue;
                                }

                                // Initialize a new ExamSessionModel
                                var examSession = new ExamSessionModel();
                                examSession.ExamPeriodId = periodId;
                                examSession.StaffId = userId;
                                examSession.CourseId = await _courseRepo.GetCourseIdByNameAsync(reader.GetValue(0).ToString());
                                examSession.CourseName = reader.GetValue(0).ToString();

                                // Validate course existence
                                if (examSession.CourseId == null)
                                {
                                    return BadRequest($"The course '{examSession.CourseName}' doesn't exist in the database");
                                }

                                if (!coursesId.Contains((Guid)examSession.CourseId))
                                {
                                    return BadRequest($"The course '{examSession.CourseName}' doesn't have '{examPeriod.ExamFormatName}' format");
                                }

                                // Validate and parse the exam date
                                if (!DateTime.TryParseExact(reader.GetValue(1).ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime examDate))
                                {
                                    return BadRequest($"Invalid date format: '{reader.GetValue(1)}'. The date should be in the format 'DD/MM/YYYY'");
                                }

                                // Check if the exam date is within the allowed scheduling period
                                if (examDate < minAllowedDate || examDate > examPeriod!.EndDate)
                                {
                                    return BadRequest($"The exam date '{String.Format("{0:dd/MM/yyyy}", examDate)}' is not allowed. Exams can be scheduled starting from '{minAllowedDate.ToString("dd/MM/yyyy")}' and end at '{String.Format("{0:dd/MM/yyyy}", examPeriod.EndDate)}'");
                                }

                                examSession.ExamDate = examDate;
                                examSession.ShiftId = await _shiftRepo.GetExamShiftIdByName(reader.GetValue(2).ToString());
                                examSession.ShiftName = reader.GetValue(2).ToString();

                                // Validate course existence
                                if (examSession.ShiftId == null)
                                {
                                    return BadRequest($"The shift '{examSession.ShiftName}' doesn't exist in the database");
                                }

                                // Store the validated exam session
                                examSessions.Add(examSession);

                            }
                        } while (reader.NextResult());
                    }
                }
                if (examSessions.Count == 0)
                {
                    return BadRequest("The uploaded file is in the wrong format. Please ensure that the first row is a header, and subsequent rows represent data. Each row should contain three properties of the exam session data: 1) Course 2) Exam Date 3) Exam Shift");
                }
                // Process the validated exam sessions
                foreach (var examSession in examSessions)
                {
                    bool result = await _examRepo.AddExamSessionAsync(examSession);

                    // Generate messages based on the processing result
                    if (result)
                    {
                        msg += $"Successfully added exam sessions for course '{examSession.CourseName}', on '{examSession.ExamDate:dd/MM/yyyy}' at '{examSession.ShiftName}'\n";
                    }
                    else
                    {
                        msg += $"Failed to add exam sessions for course '{examSession.CourseName}', on '{examSession.ExamDate:dd/MM/yyyy}' at '{examSession.ShiftName}'\n";
                    }
                }

                // Return success or error message
                return Ok(msg);
            }

            // Return a BadRequest response for an empty file
            return BadRequest("The uploaded file is empty");
        }

        // Helper
        [NonAction]
        private async Task<DataTable> GetAllExamSessionsData(Guid periodId)
        {
            DataTable dt = new DataTable();
            dt.TableName = "ExamSessionsData";
            dt.Columns.Add("ExamSessionId", typeof(Guid));
            dt.Columns.Add("CourseName", typeof(string));
            dt.Columns.Add("ExamPeriodName", typeof(string));
            dt.Columns.Add("ExamFormatCode", typeof(string));
            dt.Columns.Add("ExamFormatName", typeof(string));
            dt.Columns.Add("ExamDate", typeof(string));
            dt.Columns.Add("ShiftName", typeof(string));
            dt.Columns.Add("RoomName", typeof(string));
            dt.Columns.Add("StudentsEnrolled", typeof(int));
            dt.Columns.Add("TeacherName", typeof(string));
            dt.Columns.Add("StaffName", typeof(string));
            dt.Columns.Add("IsPassed", typeof(bool));
            dt.Columns.Add("IsPaid", typeof(bool));

            var examSessions = await _examRepo.GetExamSessionsByPeriodAsync(periodId);
            if (examSessions.Count > 0)
            {
                foreach (var exam in examSessions)
                {
                    string formattedExamDate = String.Format("{0:dd/MM/yyyy}", exam.ExamDate);
                    dt.Rows.Add(exam.ExamSessionId, exam.CourseName, exam.ExamPeriodName, exam.ExamFormatCode, exam.ExamFormatName, formattedExamDate, exam.ShiftName, exam.RoomName, exam.StudentsEnrolled, exam.TeacherName, exam.StaffName, exam.IsPassed, exam.IsPaid);
                }
            }

            dt.DefaultView.Sort = "ExamDate DESC, ShiftName ASC";
            return dt.DefaultView.ToTable();
        }

        [NonAction]
        private async Task<DateTime> GetMinAllowedSchedulingDateAsync()
        {
            // Retrieve the scheduling period setting
            var schedulingPeriodSetting = await _settingRepo.GetSettingByNameAsync("Scheduling Period");

            if (schedulingPeriodSetting == null || schedulingPeriodSetting.SettingValue == null)
            {
                // Scheduling Period setting not found or invalid.
                throw new InvalidOperationException("Scheduling Period setting not found or invalid.");
            }

            // Convert Scheduling Period setting to integer
            int schedulingPeriod = Convert.ToInt32(schedulingPeriodSetting.SettingValue);

            // Calculate the minimum allowed date for scheduling
            DateTime currentDate = DateTime.Now.Date;
            DateTime minAllowedDate = currentDate.AddDays(schedulingPeriod);

            return minAllowedDate;
        }
    }
}
