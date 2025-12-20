using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessCenter.Models
{
    public class RegisterViewModel
    {
        [Required, StringLength(100)]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; }

        [Required, EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Telefon (İsteğe bağlı)")]
        public string PhoneNumber { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Spor Salonu")]
        public int GymCenterId { get; set; }

        // Dropdown için
        public IEnumerable<SelectListItem>? GymCenters { get; set; }
    }
}
