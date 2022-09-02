using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Config
{
    public class GlobalConfigViewModel
    {
        [Display(Name = "Current Year Context for Viewing")]
        public int YearValue { get; set; }
        
        [Display(Name = "Ad-Hoc Event Feature")]
        public bool EventFeature { get; set; }

        [Display(Name = "Email Test Mode")]
        public bool EmailTestMode { get; set; }
        
        [Display(Name = "Website Theme")]
        public string Theme { get; set; }
        
        [Display(Name = "Disallow registration of duplicate students")]
        public bool DisallowDuplicateStudents { get; set; }

        [Display(Name = "Record API events")]
        public bool RecordApiEvents { get; set; }

        [Display(Name = "Display QR code in student email")]
        public bool QrInStudentEmail { get; set; }

        [Display(Name = "Max upper limit of students for this year")]
        [Range(150, 300)]
        public int MaxLimitStudentSeats { get; set; }
    }
}