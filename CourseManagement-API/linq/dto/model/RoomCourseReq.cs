using System;
using System.Collections.Generic;

namespace querry.dto.model;

public class RoomCourseReq
{
    public string RoomCourseId { get; set; } = null!;

    public string RoomId { get; set; } 

    public string CourseId { get; set; } 

    public DateTime? CreateDate { get; set; }

    public string CreateUserId { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string UpdateUserId { get; set; }


}
