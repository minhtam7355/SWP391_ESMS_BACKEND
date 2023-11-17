namespace SWP391_ESMS.Models.ViewModels
{
    public class ProctoringRequestModel
    {
        public Guid RequestId { get; set; }

        public string? RequestType { get; set; }

        public bool? RequestStatus { get; set; }

        public DateTime? RequestDate { get; set; }

        public Guid? ExamSessionId { get; set; }

        public string? ExamPeriodName { get; set; }

        public string? CourseName { get; set; }

        public DateTime? ExamDate { get; set; }

        public string? ShiftName { get; set; }

        public Guid? TeacherId { get; set; }

        public string? TeacherName { get; set; }
    }
}
