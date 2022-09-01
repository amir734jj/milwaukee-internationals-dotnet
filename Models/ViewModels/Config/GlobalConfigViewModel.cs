using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Config
{
    public class GlobalConfigViewModel
    {
        public IEnumerable<int> Years { get; set; }
        
        public int UpdatedYear { get; set; }
        
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
    }
}