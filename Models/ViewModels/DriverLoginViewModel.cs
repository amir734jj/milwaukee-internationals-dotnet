using Models.Interfaces;

namespace Models.ViewModels;

public class DriverLoginViewModel : IViewModel
{
    public string Email { get; set; }
    
    public string DriverId { get; set; }
}