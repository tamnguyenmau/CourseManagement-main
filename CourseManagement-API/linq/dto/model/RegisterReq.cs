using System.ComponentModel.DataAnnotations;

namespace linq.dto.model
{
    public class RegisterReq
    {

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 4)]
        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
