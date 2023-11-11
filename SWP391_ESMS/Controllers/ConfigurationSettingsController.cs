using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConfigurationSettingsController : ControllerBase
    {
        private readonly IConfigurationSettingRepository _settingRepo;

        public ConfigurationSettingsController(IConfigurationSettingRepository settingRepo)
        {
            _settingRepo = settingRepo;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllSettings()
        {
            try
            {
                return Ok(await _settingRepo.GetAllSettingsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetSettingById([FromRoute] Guid id)
        {
            try
            {
                return Ok(await _settingRepo.GetSettingByIdAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSetting([FromBody] ConfigurationSettingModel model)
        {
            try
            {
                bool result = await _settingRepo.UpdateSettingAsync(model);

                if (result)
                {
                    return Ok("Updated Successfully");
                }
                else
                {
                    return BadRequest("Failed to update the setting");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
