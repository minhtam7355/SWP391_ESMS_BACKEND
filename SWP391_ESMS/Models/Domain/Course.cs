using System;
using System.Collections.Generic;

namespace SWP391_ESMS.Models.Domain;

public partial class Course
{
    public Guid CourseId { get; set; }

    public string? CourseName { get; set; }

    public Guid? MajorId { get; set; }

    public virtual ICollection<ExamSession> ExamSessions { get; set; } = new List<ExamSession>();

    public virtual Major? Major { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
