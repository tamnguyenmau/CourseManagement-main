using System.ComponentModel.DataAnnotations;

namespace linq.dto.model
{
    public class ForgotReq
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

