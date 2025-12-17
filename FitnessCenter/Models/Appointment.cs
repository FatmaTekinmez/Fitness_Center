using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsApproved { get; set; }
        public decimal PriceAtBooking { get; set; }
    }
}
