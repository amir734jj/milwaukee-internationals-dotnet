using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Enums;
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
        
        public string DisplayId { get; set; }
        
        [Display(Name = "Capacity")]
        
        [Range(1, 7)]
        public int Capacity { get; set; } = 1;
        
        [Display(Name="Require Navigator")]
        public bool RequireNavigator { get; set; }
        
        [Display(Name="Navigator fullname")]
        public string Navigator { get; set; }
        
        [Display(Name="Role")]
        public RolesEnum Role { get; set; }
        
        public List<Student> Students { get; set; }
        
        /// <summary>
        /// Optional
        /// </summary>
        public int? HostRefId { get; set; }

        [ForeignKey("HostRefId")]
        public Host Host { get; set; }
        
        public bool IsPressent { get; set; }
    }
}