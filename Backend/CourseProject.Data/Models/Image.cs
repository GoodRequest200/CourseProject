using System;
using System.Collections.Generic;

namespace CourseProject.Data.Models;

public partial class Image
{
    public int Id { get; set; }

    public string FilePath { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }
}
