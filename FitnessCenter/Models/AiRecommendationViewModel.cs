namespace FitnessCenter.Models
{
    public class AiRecommendationViewModel
    {
        public string Goal { get; set; }          // kilo verme, kas kazanma...
        public string Level { get; set; }         // başlangıç, orta, ileri
        public string Preferences { get; set; }   // ekipman, sevmediği hareketler vb.

        public string Recommendation { get; set; } // Gemini’den gelen cevap
    }
}
