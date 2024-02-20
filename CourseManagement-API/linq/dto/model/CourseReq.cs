using System;
using System.Collections.Generic;

namespace querry.dto.model;

public partial class CourseReq
{
    public string CourseId { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public string? Description { get; set; }

    public string? TeacherId { get; set; }

    public decimal? Price { get; set; }

    public int? MaxStudentQuantity { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreatedUserId { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdatedUserId { get; set; }


}
