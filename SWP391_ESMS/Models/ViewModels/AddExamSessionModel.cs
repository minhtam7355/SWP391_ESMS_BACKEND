namespace SWP391_ESMS.Models.ViewModels
{
    public class AddExamSessionModel
    {
        public Guid ExamSessionId { get; set; }

        public Guid? CourseId { get; set; }

        public DateTime? ExamDate { get; set; }

        public Guid? RoomId { get; set; }

        public Guid? TeacherId { get; set; }

        public Guid? StaffId { get; set; }

        public Guid? ShiftId { get; set; }

    }
}
