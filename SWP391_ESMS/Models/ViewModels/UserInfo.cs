namespace SWP391_ESMS.Models.ViewModels
{
    public class UserInfo
    {
        public Guid UserId { get; set; }

        public string? Username { get; set; }

        public string? PasswordHash { get; set; }

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? HomeAddress { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? ProfilePicture { get; set; }

        public string? Role { get; set; }
    }
}
