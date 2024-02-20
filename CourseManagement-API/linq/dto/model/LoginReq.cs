using System.ComponentModel.DataAnnotations;

namespace linq.dto.model
{
    public class LoginReq
    {
        [Required]
        //[EmailAddress]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }


    }
}
