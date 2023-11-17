using System;
using System.Collections.Generic;

namespace SWP391_ESMS.Models.Domain;

public partial class ExamPeriod
{
    public Guid ExamPeriodId { get; set; }

    public string? ExamPeriodName { get; set; }

    public Guid? ExamFormatId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual ExamFormat? ExamFormat { get; set; }

    public virtual ICollection<ExamSession> ExamSessions { get; set; } = new List<ExamSession>();
}
