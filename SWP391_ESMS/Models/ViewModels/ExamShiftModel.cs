namespace SWP391_ESMS.Models.ViewModels
{
    public class ExamShiftModel
    {
        public Guid ShiftId { get; set; }

        public string? ShiftName { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

    }
}
