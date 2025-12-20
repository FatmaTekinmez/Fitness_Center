using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }
        public string SpecialtyIds { get; set; } = "";
        public int FitnessCenterId { get; set; }
        public GymCenter GymCenter { get; set; }
        public ICollection<TrainerService> TrainerServices { get; set; }
        public ICollection<TrainerAvailability> Availabilities { get; set; }
    }
}
