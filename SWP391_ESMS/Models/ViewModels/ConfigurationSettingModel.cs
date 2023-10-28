namespace SWP391_ESMS.Models.ViewModels
{
    public class ConfigurationSettingModel
    {
        public Guid SettingId { get; set; }

        public string? SettingName { get; set; }

        public decimal? SettingValue { get; set; }

        public string? SettingDescription { get; set; }
    }
}
