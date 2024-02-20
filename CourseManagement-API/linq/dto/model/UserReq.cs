namespace querry.dto.model
{
    public class UserReq
    {
        public string Id { get; set; } 
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }      
        public bool? IsTeacher { get; set; }
        public bool? IsStudent { get; set; }
        public DateTime? LockDate { get; set;}
        public string? Image {  get; set; }

        public bool LockoutEnabled { get; set; }
    }
}
