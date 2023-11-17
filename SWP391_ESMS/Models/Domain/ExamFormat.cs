using System;
using System.Collections.Generic;

namespace SWP391_ESMS.Models.Domain;

public partial class ExamFormat
{
    public Guid ExamFormatId { get; set; }

    public string? ExamFormatCode { get; set; }

    public string? ExamFormatName { get; set; }

    public virtual ICollection<ExamPeriod> ExamPeriods { get; set; } = new List<ExamPeriod>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
