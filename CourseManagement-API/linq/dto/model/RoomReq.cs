using System;
using System.Collections.Generic;

namespace querry.dto.model;

public class RoomReq
{
    public string RoomId { get; set; } = null!;

    public string RoomName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreatedUserId { get; set; }
    
    public DateTime? UpdateDate { get; set; }

    public string? UpdateUserId { get; set; }
}
