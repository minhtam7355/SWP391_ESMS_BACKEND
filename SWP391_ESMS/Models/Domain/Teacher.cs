using System;
using System.Collections.Generic;

namespace SWP391_ESMS.Models.Domain;

public partial class Teacher
{
    public Guid TeacherId { get; set; }

    public string? Username { get; set; }

    public string? PasswordHash { get; set; }

    public string? Email { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? HomeAddress { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? ProfilePicture { get; set; }

    public Guid? MajorId { get; set; }

    public virtual ICollection<ExamSession> ExamSessions { get; set; } = new List<ExamSession>();

    public virtual Major? Major { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
