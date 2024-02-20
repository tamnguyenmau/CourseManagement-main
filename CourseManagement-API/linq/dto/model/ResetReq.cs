using System.ComponentModel.DataAnnotations;

namespace linq.dto.model
{
    public class ResetReq
    {
        [Required]
        //[EmailAddress]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
