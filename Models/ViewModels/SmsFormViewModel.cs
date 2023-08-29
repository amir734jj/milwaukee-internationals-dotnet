using System.ComponentModel.DataAnnotations;
using Models.Interfaces;

namespace Models.ViewModels;

public class SmsFormViewModel : IViewModel
{
    public bool Admin { get; set; }

    public bool Users { get; set; }
        
    public bool Students { get; set; }
        
    public bool Drivers { get; set; }
        
    public bool Hosts { get; set; }
        
    public string Message { get; set; }

    public bool Status { get; set; } = true;
        
    public int AdminCount { get; set; }
        
    public int UserCount { get; set; }
        
    public int StudentCount { get; set;  }
        
    public int DriverCount { get; set; }
        
    public int HostCount { get; set; }

    [Display(Name = "Additional Recipients")]
    public string AdditionalRecipients { get; set; }
}