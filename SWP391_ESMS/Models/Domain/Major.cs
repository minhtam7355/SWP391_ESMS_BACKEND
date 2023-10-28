using System;
using System.Collections.Generic;

namespace SWP391_ESMS.Models.Domain;

public partial class Major
{
    public Guid MajorId { get; set; }

    public string? MajorName { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
