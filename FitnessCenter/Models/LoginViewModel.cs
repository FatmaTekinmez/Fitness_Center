using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
