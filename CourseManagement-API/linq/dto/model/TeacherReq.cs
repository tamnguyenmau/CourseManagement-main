using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace linq.dto.model
{
    public class TeacherReq
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? Password { get; set; }
        public bool IsTeacher { get; set; }
        public DateTime? LockDate { get; set; }
        public bool? LockoutEnabled { get; set; }

    }
}
