using System.ComponentModel.DataAnnotations;

namespace CharityApp.Models
{
    public class RegisterViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

    }
}
