namespace SWP391_ESMS.Models.ViewModels
{
    public class StudentModel
    {
        public Guid StudentId { get; set; }

        public string? Username { get; set; }

        public string? PasswordHash { get; set; }

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? HomeAddress { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? ProfilePicture { get; set; }

        public Guid? MajorId { get; set; }

        public virtual MajorModel? Major { get; set; }

        public virtual ICollection<CourseModel> Courses { get; set; } = new List<CourseModel>();

        public virtual ICollection<ExamSessionModel> ExamSessions { get; set; } = new List<ExamSessionModel>();
    }
}
