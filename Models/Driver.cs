using System.ComponentModel.DataAnnotations;
using Models.Interfaces;

namespace Models
{
    public class Driver : IPerson
    {
        [Key]
        public int Id { get; set; }
        
        [Display(Name="Email")]
        public string Email { get; set; }
        
        [Display(Name="Phone")]
        public string Phone { get; set; }
        
        [Display(Name="Fullname")]
        public string Fullname { get; set; }

        [Display(Name = "Capacity")]
        [Range(1, 7)]
        public int Capacity { get; set; } = 1;
        
        [Display(Name="Require Navigator")]
        public bool RequireNavigator { get; set; }
        
        [Display(Name="Navigator")]
        public string Navigator { get; set; }
    }
}