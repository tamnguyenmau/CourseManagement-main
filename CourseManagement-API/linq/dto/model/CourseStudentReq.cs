namespace querry.dto.model
{
    public class CourseStudentReq
    {
        public string CourseStudentId { get; set; } = null!;

        public string CourseId { get; set; } 

        public string StudentId { get; set; }

        public DateTime CreateDate {  get; set; }
        
        public string CreateUserId { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdateUserId { get; set; }
    }
}
