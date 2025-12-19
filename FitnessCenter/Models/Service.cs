using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }
        public int DurationMinutes { get; set; }
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public int FitnessCenterId { get; set; }
        public int GymCenterId { get; set; }
        [ValidateNever]
        public GymCenter GymCenter { get; set; }
        [ValidateNever]
        public ICollection<TrainerService> TrainerServices { get; set; }
        [ValidateNever]
        public ICollection<Appointment> Appointments { get; set; }
    }
}
