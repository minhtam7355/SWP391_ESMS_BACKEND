namespace SWP391_ESMS.Models.ViewModels
{
    public class StudentModel
    {
        public Guid StudentId { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? HomeAddress { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? ProfilePicture { get; set; }

        public Guid? MajorId { get; set; }

        public string? MajorName { get; set; }

    }
}
