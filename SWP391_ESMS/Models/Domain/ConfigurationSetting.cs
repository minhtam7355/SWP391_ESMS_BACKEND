using System;
using System.Collections.Generic;

namespace SWP391_ESMS.Models.Domain;

public partial class ConfigurationSetting
{
    public Guid SettingId { get; set; }

    public string? SettingName { get; set; }

    public decimal? SettingValue { get; set; }

    public string? SettingDescription { get; set; }
}
