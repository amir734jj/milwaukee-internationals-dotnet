using System;

namespace Models.Constants;

public class GlobalConfigs
{
    public DateTimeOffset LastModified { get; set; } = DateTimeOffset.Now;

    public bool EventFeature { get; set; }

    public int YearValue { get; set; } = DateTime.UtcNow.Year;

    public bool EmailTestMode { get; set; }

    /// <summary>
    ///  Current website theme
    /// </summary>
    public string Theme { get; set; }

    public bool DisallowDuplicateStudents { get; set; }
    public bool RecordApiEvents { get; set; }
    public bool QrInStudentEmail { get; set; }
        
    /// <summary>
    /// After the max limit, the sign up page for student closes.
    /// </summary>
    public int MaxLimitStudentSeats { get; set; } = 160;
        
    public DateTime TourDate { get; set; } = DateTime.Now;
        
    public string TourAddress { get; set; }     
        
    public string TourLocation { get; set; }
    
    public bool LocationWizardFeature { get; set; }
}