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
        public GymCenter GymCenter { get; set; }
        public ICollection<TrainerService> TrainerServices { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
