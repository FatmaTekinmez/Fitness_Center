using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FitnessCenter.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public int? FitnessCenterId { get; set; }
        public GymCenter GymCenter { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
