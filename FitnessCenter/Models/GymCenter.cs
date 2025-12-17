using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    public class GymCenter
    {
        public int Id { get; set; }

        [Required, StringLength(60)]
        public string Name { get; set; }
        [StringLength(120)]
        public string Address { get; set; }
        [StringLength(30)]
        public string WorkingHours { get; set; }
        public ICollection<Service> Services { get; set; }
        public ICollection<Trainer> Trainers { get; set; }
    }
}
