using System;
using System.Collections.Generic;

namespace SWP391_ESMS.Models.Domain;

public partial class ExamShift
{
    public Guid ShiftId { get; set; }

    public string? ShiftName { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public virtual ICollection<ExamSession> ExamSessions { get; set; } = new List<ExamSession>();
}
