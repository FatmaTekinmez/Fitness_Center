using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FitnessCenter.Models.ViewModels
{
    public class AppointmentCreateViewModel
    {
        [Required]
        public int GymCenterId { get; set; }

        [Required]
        [Display(Name = "Antrenör")]
        public int TrainerId { get; set; }

        [Required]
        [Display(Name = "Hizmet")]
        public int ServiceId { get; set; }

        [Required]
        [Display(Name = "Başlangıç Zamanı")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "Bitiş Zamanı")]
        public DateTime EndTime { get; set; }

        public string GymCenterName { get; set; }

        // Dropdownlar için
        [ValidateNever]
        public IEnumerable<Trainer>? Trainers { get; set; }
    }
}
