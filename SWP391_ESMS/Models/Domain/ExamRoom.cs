using System;
using System.Collections.Generic;

namespace SWP391_ESMS.Models.Domain;

public partial class ExamRoom
{
    public Guid RoomId { get; set; }

    public string? RoomName { get; set; }

    public virtual ICollection<ExamSession> ExamSessions { get; set; } = new List<ExamSession>();
}
