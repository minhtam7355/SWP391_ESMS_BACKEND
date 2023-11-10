using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public class ConfigurationSettingRepository : IConfigurationSettingRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public ConfigurationSettingRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<ConfigurationSettingModel>> GetAllSettingsAsync()
        {
            var settings = await _dbContext.ConfigurationSettings.ToListAsync();
            return _mapper.Map<List<ConfigurationSettingModel>>(settings);
        }

        public async Task<ConfigurationSettingModel> GetSettingByIdAsync(Guid id)
        {
            var setting = await _dbContext.ConfigurationSettings.FindAsync(id);
            return _mapper.Map<ConfigurationSettingModel>(setting);
        }

        public async Task<ConfigurationSettingModel?> GetSettingByNameAsync(string? settingName)
        {
            var setting = await _dbContext.ConfigurationSettings.FirstOrDefaultAsync(c => c.SettingName == settingName);
            if (setting == null) { return null; }
            return _mapper.Map<ConfigurationSettingModel>(setting);
        }

        public async Task<bool> UpdateSettingAsync(ConfigurationSettingModel model)
        {
            var existingSetting = await _dbContext.ConfigurationSettings.FindAsync(model.SettingId);

            if (existingSetting != null)
            {
                _mapper.Map(model, existingSetting);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
