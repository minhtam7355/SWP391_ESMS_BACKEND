using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestRepository _requestRepo;
        private readonly IExamSessionRepository _examRepo;
        private readonly IExamRoomRepository _roomRepo;
        private readonly IConfigurationSettingRepository _settingRepo;

        public RequestsController(IRequestRepository requestRepo, IExamSessionRepository examRepo, IExamRoomRepository roomRepo, IConfigurationSettingRepository settingRepo)
        {
            _requestRepo = requestRepo;
            _examRepo = examRepo;
            _roomRepo = roomRepo;
            _settingRepo = settingRepo;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllRequests()
        {
            try
            {
                return Ok(await _requestRepo.GetAllRequestsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getallpendingproctoringrequests")]
        public async Task<IActionResult> GetAllPendingProctoringRequests()
        {
            try
            {
                return Ok(await _requestRepo.GetAllPendingProctoringRequestsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getallpendingunproctoringrequests")]
        public async Task<IActionResult> GetAllPendingUnproctoringRequests()
        {
            try
            {
                return Ok(await _requestRepo.GetAllPendingUnproctoringRequestsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getproctoringrequestsbyteacher")]
        public async Task<IActionResult> GetProctoringRequestsByTeacher()
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
                var requests = await _requestRepo.GetProctoringRequestsByTeacherAsync(teacherId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getunproctoringrequestsbyteacher")]
        public async Task<IActionResult> GetUnproctoringRequestsByTeacher()
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
                var requests = await _requestRepo.GetUnproctoringRequestsByTeacherAsync(teacherId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetRequestById([FromRoute] Guid id)
        {
            try
            {
                var request = await _requestRepo.GetRequestByIdAsync(id);
                return Ok(request);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("createproctoringrequest/{examSessionId}")]
        public async Task<IActionResult> AddProctoringRequest([FromRoute] Guid examSessionId)
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

                bool result = await _requestRepo.AddProctoringRequestAsync(examSessionId, teacherId);

                if (result)
                {
                    return Ok("Added Successfully");
                }
                else
                {
                    return BadRequest("Failed to add the proctoring request");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("createunproctoringrequest/{examSessionId}")]
        public async Task<IActionResult> AddUnproctoringRequest([FromRoute] Guid examSessionId)
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

                DateTime minAllowedDate = await GetMinAllowedCancelProctorDateAsync(examSessionId);

                if (DateTime.Now.Date >= minAllowedDate)
                {
                    return BadRequest($"Unproctoring request can only be sent before '{minAllowedDate.ToString("dd/MM/yyyy")}'");
                }

                bool result = await _requestRepo.AddUnproctoringRequestAsync(examSessionId, teacherId);

                if (result)
                {
                    return Ok("Added Successfully");
                }
                else
                {
                    return BadRequest("Failed to add the unproctoring request");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("approveproctoringrequest/{id}")]
        public async Task<IActionResult> ApproveProctoringRequest([FromRoute] Guid id)
        {
            try
            {
                var request = await _requestRepo.GetRequestByIdAsync(id);
                var examSession = await _examRepo.GetExamSessionByIdAsync(request.ExamSessionId ?? Guid.Empty);
                if (examSession.RoomId == null)
                {
                    bool areAvailableRooms = await _roomRepo.GetAvailableRoomsAsync(examSession.ExamDate, examSession.ShiftId);

                    if (!areAvailableRooms)
                    {
                        return BadRequest("No available rooms for the proctoring request");
                    }
                }

                bool result = await _requestRepo.ApproveProctoringRequestAsync(id);
                if (result)
                {
                    return Ok("Approved Successfully");
                }
                else
                {
                    return BadRequest("Failed to approve the request");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("rejectproctoringrequest/{id}")]
        public async Task<IActionResult> RejectProctoringRequest([FromRoute] Guid id)
        {
            try
            {
                bool result = await _requestRepo.RejectProctoringRequestAsync(id);
                if (result)
                {
                    return Ok("Rejected Successfully");
                }
                else
                {
                    return BadRequest("Failed to reject the request");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("approveunproctoringrequest/{id}")]
        public async Task<IActionResult> ApproveUnproctoringRequest([FromRoute] Guid id)
        {
            try
            {
                bool result = await _requestRepo.ApproveUnproctoringRequestAsync(id);
                if (result)
                {
                    return Ok("Approved Successfully");
                }
                else
                {
                    return BadRequest("Failed to approve the request");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("rejectunproctoringrequest/{id}")]
        public async Task<IActionResult> RejectUnproctoringRequest([FromRoute] Guid id)
        {
            try
            {
                bool result = await _requestRepo.RejectUnproctoringRequestAsync(id);
                if (result)
                {
                    return Ok("Rejected Successfully");
                }
                else
                {
                    return BadRequest("Failed to reject the request");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [NonAction]
        private async Task<DateTime> GetMinAllowedCancelProctorDateAsync(Guid examSessionId)
        {
            var cancellationNoticeSetting = await _settingRepo.GetSettingByNameAsync("Cancellation Notice");

            if (cancellationNoticeSetting == null || cancellationNoticeSetting.SettingValue == null)
            {
                throw new InvalidOperationException("Cancellation Notice setting not found or invalid.");
            }

            int cancellationNotice = Convert.ToInt32(cancellationNoticeSetting.SettingValue);

            var examSession = await _examRepo.GetExamSessionByIdAsync(examSessionId);

            if (examSession == null || examSession.ExamDate == null)
            {
                throw new InvalidOperationException("Exam session not found or invalid.");
            }

            DateTime minAllowedDate = examSession.ExamDate.Value.AddDays(-cancellationNotice);

            return minAllowedDate;
        }
    }
}
