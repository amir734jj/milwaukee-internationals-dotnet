using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Models.Interfaces;

namespace Models
{
    public class Student : IPerson
    {
        [Key]
        public int Id { get; set; }
        
        public string Fullname { get; set; }
        
        public string Major { get; set; }
        
        public string University { get; set; }
        
        public string Email { get; set; }
        
        public string Phone { get; set; }
        
        public string Country { get; set; }

        [Display(Name = "Tell us some of your interests")]
        public string Interests { get; set; }
        
        public string DisplayId { get; set; }
        
        [Display(Name="Need a car seat?")]
        public bool NeedCarSeat { get; set; }
        
        [Display(Name="Halal or Kosher food")]
        public bool KosherFood { get; set; }
        
        public bool IsPressent { get; set; }
        
        /// <summary>
        /// Optional
        /// </summary>
        public int? DriverRefId { get; set; }

        [ForeignKey("DriverRefId")]
        public Driver Driver { get; set; }
    }
}