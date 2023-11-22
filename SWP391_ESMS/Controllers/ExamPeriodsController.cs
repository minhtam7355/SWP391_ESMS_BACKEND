using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamPeriodsController : ControllerBase
    {
        private readonly IExamPeriodRepository _periodRepo;
        private readonly IExamSessionRepository _examRepo;
        private readonly IConfigurationSettingRepository _settingRepo;

        public ExamPeriodsController(IExamPeriodRepository periodRepo, IExamSessionRepository examRepo, IConfigurationSettingRepository settingRepo)
        {
            _periodRepo = periodRepo;
            _examRepo = examRepo;
            _settingRepo = settingRepo;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllExamPeriods()
        {
            try
            {
                return Ok(await _periodRepo.GetAllExamPeriodsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetExamPeriodById([FromRoute] Guid id)
        {
            try
            {
                var examPeriod = await _periodRepo.GetExamPeriodByIdAsync(id);
                return Ok(examPeriod);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddExamPeriod([FromBody] ExamPeriodModel model)
        {
            try
            {
                if (model.StartDate <= DateTime.Now.Date)
                {
                    return BadRequest($"The exam period start date '{String.Format("{0:dd/MM/yyyy}", model.StartDate)}' is not allowed. Exam period start date can be scheduled starting from '{String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(model.StartDate).AddDays(1))}'");
                }
                if (model.EndDate <= model.StartDate)
                {
                    return BadRequest($"The exam period end date '{String.Format("{0:dd/MM/yyyy}", model.EndDate)}' is not allowed. Exam period end date can not be before or the same as the start date '{String.Format("{0:dd/MM/yyyy}", model.StartDate)}'");
                }

                bool result = await _periodRepo.AddExamPeriodAsync(model);

                if (result)
                {
                    return Ok("Added Successfully");
                }
                else
                {
                    return BadRequest("Failed to add the exam period");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateExamPeriod([FromBody] ExamPeriodModel model)
        {
            try
            {
                var examPeriod = await _periodRepo.GetExamPeriodByIdAsync(model.ExamPeriodId);
                if (examPeriod == null)
                {
                    return NotFound();
                }
                if (model.StartDate != examPeriod.StartDate)
                {
                    if (model.StartDate >= model.EndDate)
                    {
                        return BadRequest();
                    }

                    var examSessions = await _examRepo.GetExamSessionsByPeriodAsync(model.ExamPeriodId);
                    foreach (var examSession in examSessions)
                    {
                        if (examSession.ExamDate < model.StartDate)
                        {
                            return BadRequest();
                        }
                    }
                }
                if (model.EndDate != examPeriod.EndDate)
                {
                    if (model.EndDate <= model.StartDate)
                    {
                        return BadRequest();
                    }

                    var examSessions = await _examRepo.GetExamSessionsByPeriodAsync(model.ExamPeriodId);
                    foreach (var examSession in examSessions)
                    {
                        if (examSession.ExamDate > model.EndDate)
                        {
                            return BadRequest();
                        }
                    }
                }
                bool result = await _periodRepo.UpdateExamPeriodAsync(model);

                if (result)
                {
                    return Ok("Updated Successfully");
                }
                else
                {
                    return BadRequest("Failed to update the exam period because there exist an exam session outside of the updated exam period");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteExamPeriod([FromRoute] Guid id)
        {
            try
            {
                bool result = await _periodRepo.DeleteExamPeriodAsync(id);

                if (result)
                {
                    return Ok("Deleted Successfully");
                }
                else
                {
                    return BadRequest("Failed to delete the exam period");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
