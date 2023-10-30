namespace SWP391_ESMS.Models.ViewModels
{
    public class ChangePasswordModel
    {
        public string? CurrentPassword { get; set; }

        public string? NewPassword { get; set; }
        
        public string? ConfirmPassword { get; set; }
    }
}
