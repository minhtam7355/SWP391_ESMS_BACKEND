namespace SWP391_ESMS.Models.ViewModels
{
    public class ExamPeriodModel
    {
        public Guid ExamPeriodId { get; set; }

        public string? ExamPeriodName { get; set; }

        public Guid? ExamFormatId { get; set; }

        public string? ExamFormatCode { get; set; }

        public string? ExamFormatName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
