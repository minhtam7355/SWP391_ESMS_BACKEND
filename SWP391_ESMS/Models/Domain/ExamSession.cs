using System;
using System.Collections.Generic;

namespace SWP391_ESMS.Models.Domain;

public partial class ExamSession
{
    public Guid ExamSessionId { get; set; }

    public Guid? CourseId { get; set; }

    public Guid? ExamPeriodId { get; set; }

    public DateTime? ExamDate { get; set; }

    public Guid? ShiftId { get; set; }

    public Guid? RoomId { get; set; }

    public int? StudentsEnrolled { get; set; }

    public Guid? TeacherId { get; set; }

    public Guid? StaffId { get; set; }

    public bool? IsPassed { get; set; }

    public bool? IsPaid { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ExamPeriod? ExamPeriod { get; set; }

    public virtual ICollection<ProctoringRequest> ProctoringRequests { get; set; } = new List<ProctoringRequest>();

    public virtual ExamRoom? Room { get; set; }

    public virtual ExamShift? Shift { get; set; }

    public virtual Staff? Staff { get; set; }

    public virtual Teacher? Teacher { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
