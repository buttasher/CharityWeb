using System.ComponentModel.DataAnnotations;

namespace CharityApp.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string Email { get; set; }
    }

}
