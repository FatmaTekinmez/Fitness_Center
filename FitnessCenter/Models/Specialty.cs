using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    public class Specialty
    {
        public int Id { get; set; }
        [Required, StringLength(40)]
        public string Name { get; set; }
    }
}
