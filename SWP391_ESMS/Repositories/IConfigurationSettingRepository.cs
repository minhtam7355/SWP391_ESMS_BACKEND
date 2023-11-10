using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IConfigurationSettingRepository
    {
        public Task<List<ConfigurationSettingModel>> GetAllSettingsAsync();

        public Task<ConfigurationSettingModel> GetSettingByIdAsync(Guid id);

        public Task<Boolean> UpdateSettingAsync(ConfigurationSettingModel model);

        public Task<ConfigurationSettingModel?> GetSettingByNameAsync(string? settingName);
    }
}
