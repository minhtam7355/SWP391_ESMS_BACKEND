using System;
using System.Collections.Generic;

namespace SWP391_ESMS.Models.Domain;

public partial class ProctoringRequest
{
    public Guid RequestId { get; set; }

    public string? RequestType { get; set; }

    public bool? RequestStatus { get; set; }

    public DateTime? RequestDate { get; set; }

    public Guid? ExamSessionId { get; set; }

    public Guid? TeacherId { get; set; }

    public virtual ExamSession? ExamSession { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
