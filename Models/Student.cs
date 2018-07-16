using System.ComponentModel.DataAnnotations;
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
        
        public string Interests { get; set; }
        
        public string DisplayId { get; set; }
        
        public bool NeedCarSeat { get; set; }
        
        public bool KosherFood { get; set; }
        
        public bool IsPressent { get; set; }
    }
}