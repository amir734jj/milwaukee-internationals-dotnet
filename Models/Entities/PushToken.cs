using System.ComponentModel.DataAnnotations;
using Models.Interfaces;

namespace Models.Entities;

public class PushToken : IEntity
{
    [Key]
    public int Id { get; set; }
    
    public int DriverId { get; set; }
    
    public Driver Driver { get; set; }
    
    public string Token { get; set; }
}