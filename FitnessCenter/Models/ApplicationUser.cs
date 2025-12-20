using FitnessCenter.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public int? GymCenterId { get; set; }
    public GymCenter GymCenter { get; set; }
    public ICollection<Appointment> Appointments { get; set; }

    // 🔽 YENİ ALANLAR
    [Range(100, 250)]
    public int? HeightCm { get; set; }   // Boy (cm)

    [Range(30, 250)]
    public double? WeightKg { get; set; } // Kilo (kg)

    [NotMapped]
    public double? Bmi
    {
        get
        {
            if (HeightCm.HasValue && WeightKg.HasValue && HeightCm.Value > 0)
            {
                var h = HeightCm.Value / 100.0;
                return WeightKg.Value / (h * h);
            }
            return null;
        }
    }


}
