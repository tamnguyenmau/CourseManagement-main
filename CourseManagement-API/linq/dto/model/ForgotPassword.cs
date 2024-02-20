using System.ComponentModel.DataAnnotations;

namespace linq.dto.model
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
