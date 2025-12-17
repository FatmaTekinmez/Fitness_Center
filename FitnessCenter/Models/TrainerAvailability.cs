using System;

namespace FitnessCenter.Models
{
    public class TrainerAvailability
    {
        public int Id { get; set; }
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }
    }
}
