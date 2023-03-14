using System.ComponentModel.DataAnnotations;
using Models.Interfaces;

namespace Models.Entities;

public class LocationMapping : IYearlyEntity
{
    [Key]
    public int Id { get; set; }
    
    public int SourceId { get; set; }
    
    [Display(Name = "Source Location")]
    public Location Source { get; set; }
    
    public int SinkId { get; set; }
    
    [Display(Name = "Target Location")]
    public Location Sink { get; set; }

    public int Year { get; set; }
}