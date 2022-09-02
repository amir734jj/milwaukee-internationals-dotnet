using System;

namespace Models.Constants
{
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
        public int MaxLimitStudentSeats { get; set; } = 160;
    }
}