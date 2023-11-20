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
        private readonly IConfigurationSettingRepository _settingRepo;

        public ExamPeriodsController(IExamPeriodRepository periodRepo, IConfigurationSettingRepository settingRepo)
        {
            _periodRepo = periodRepo;
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

        [HttpPost("create")]
        public async Task<IActionResult> AddExamPeriod([FromBody] ExamPeriodModel model)
        {
            try
            {
                DateTime minAllowedDate = await GetMinAllowedDateAsync();

                if (model.StartDate < minAllowedDate)
                {
                    return BadRequest($"The exam period start date '{String.Format("{0:dd/MM/yyyy}", model.StartDate)}' is not allowed. Exam period start date can be scheduled starting from '{minAllowedDate.ToString("dd/MM/yyyy")}'");
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
                DateTime minAllowedDate = await GetMinAllowedDateAsync();

                if (model.StartDate < minAllowedDate)
                {
                    return BadRequest($"The exam period start date '{String.Format("{0:dd/MM/yyyy}", model.StartDate)}' is not allowed. Exam period start date can be scheduled starting from '{minAllowedDate.ToString("dd/MM/yyyy")}'");
                }
                if (model.EndDate <= model.StartDate)
                {
                    return BadRequest($"The exam period end date '{String.Format("{0:dd/MM/yyyy}", model.EndDate)}' is not allowed. Exam period end date can not be before or the same as the start date '{String.Format("{0:dd/MM/yyyy}", model.StartDate)}'");
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

        [NonAction]
        private async Task<DateTime> GetMinAllowedDateAsync()
        {
            // Retrieve the scheduling period setting
            var schedulingPeriodSetting = await _settingRepo.GetSettingByNameAsync("Scheduling Period");
            int schedulingPeriod = Convert.ToInt32(schedulingPeriodSetting!.SettingValue);

            // Calculate the minimum allowed date
            DateTime currentDate = DateTime.Now.Date;
            DateTime minAllowedDate = currentDate.AddDays(schedulingPeriod);

            return minAllowedDate;
        }
    }
}
