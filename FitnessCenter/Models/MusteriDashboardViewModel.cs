using System.Collections.Generic;

namespace FitnessCenter.Models
{
    public class MusteriDashboardViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }

        public int? HeightCm { get; set; }
        public double? WeightKg { get; set; }
        public double? BMI { get; set; }
        public string BmiCategory { get; set; }


        // 🔽 yeni alan
        public string GymCenterName { get; set; }

        // İstersen burada randevuların özetini, seçili spor salonunu vs. de gösterebilirsin
    }
}
