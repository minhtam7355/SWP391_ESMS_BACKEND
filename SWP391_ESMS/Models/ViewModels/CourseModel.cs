namespace SWP391_ESMS.Models.ViewModels
{
    public class CourseModel
    {
        public Guid CourseId { get; set; }

        public string? CourseName { get; set; }

        public Guid? MajorId { get; set; }

        public string? MajorName { get; set; }

    }
}
