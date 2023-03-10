using System.ComponentModel.DataAnnotations;
using Models.Interfaces;

namespace Models.Entities;

public class Location : IYearlyEntity
{
    [Key]
    public int Id { get; set; }
    
    public int Rank { get; set; }
    
    public int Year { get; set; }
    
    public string Name { get; set; }
    
    public string Address { get; set; }
    
    public string Description { get; set; }
}