using System.ComponentModel.DataAnnotations;

namespace CharityApp.Models
{
    public class ResetPasswordViewModel
    {
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
