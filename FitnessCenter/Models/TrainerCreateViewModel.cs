using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FitnessCenter.Models.ViewModels
{
    public class TrainerCreateViewModel
    {
        [Required, StringLength(50)]
        public string Name { get; set; }

        [Required]
        public int FitnessCenterId { get; set; }

        [Required(ErrorMessage = "En az bir hizmet seçmelisiniz.")]
        public int[] SelectedServiceIds { get; set; }

        [Required]
        public DateTime AvailableFrom { get; set; }

        [Required]
        public DateTime AvailableTo { get; set; }

        // 🔽 Bunlar sadece ekranda liste göstermek için, input değil!
        [ValidateNever]
        public IEnumerable<GymCenter>? GymCenters { get; set; }

        [ValidateNever]
        public IEnumerable<Service>? Services { get; set; }
    }
}
