using System;
using System.Collections.Generic;

namespace CourseProject.Data.Models;

public partial class Event
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string? MainImage { get; set; }

    public DateOnly? EventDate { get; set; }

    public bool IsCompleted { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}
