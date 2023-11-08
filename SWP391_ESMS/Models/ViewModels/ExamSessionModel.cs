namespace SWP391_ESMS.Models.ViewModels
{
    public class ExamSessionModel
    {
        public Guid ExamSessionId { get; set; }

        public Guid? CourseId { get; set; }

        public string? CourseName { get; set; }
        
        public string? ExamFormat { get; set; }

        public DateTime? ExamDate { get; set; }

        public Guid? ShiftId { get; set; }

        public string? ShiftName { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public Guid? RoomId { get; set; }

        public string? RoomName { get; set; }

        public int? StudentsEnrolled { get; set; }

        public Guid? TeacherId { get; set; }

        public string? TeacherName { get; set; }

        public Guid? StaffId { get; set; }

        public string? StaffName { get; set; }

        public bool? IsPassed { get; set; }

        public bool? IsPaid { get; set; }

    }
}
