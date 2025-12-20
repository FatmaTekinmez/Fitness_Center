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

        public string GymCenterName { get; set; }

        // Kullanıcının bağlı olduğu spor salonunun Id'si
        public int? GymCenterId { get; set; }
    }
}
