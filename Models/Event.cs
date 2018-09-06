using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models.Interfaces;

namespace Models
{
    public class Event : IBasicModel
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public DateTime DateTime { get; set; }
        
        public string Address { get; set; }
        
        public List<Student> Student { get; set; }
    }
}