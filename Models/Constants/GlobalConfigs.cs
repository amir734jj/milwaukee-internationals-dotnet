using System;
using Models.ViewModels.Config;
using Newtonsoft.Json;

namespace Models.Constants
{
    public class GlobalConfigs
    {
        public DateTimeOffset LastModified { get; set; } = DateTimeOffset.Now;

        public bool EventFeature { get; private set; }

        public int YearValue { get; set; } = DateTime.UtcNow.Year;

        public bool EmailTestMode { get; private set; }

        /// <summary>
        ///  Current website theme
        /// </summary>
        public string CurrentTheme = "default";

        public void UpdateGlobalConfigs(GlobalConfigViewModel globalConfigViewModel)
        {
            YearValue = globalConfigViewModel.UpdatedYear;
            EventFeature = globalConfigViewModel.EventFeature;
            EmailTestMode = globalConfigViewModel.EmailTestMode;
            CurrentTheme = globalConfigViewModel.Theme;
            DisallowDuplicateStudents = globalConfigViewModel.DisallowDuplicateStudents;
            RecordApiEvents = globalConfigViewModel.RecordApiEvents;
            QrInStudentEmail = globalConfigViewModel.QrInStudentEmail;
        }

        public bool DisallowDuplicateStudents { get; set; }
        public bool RecordApiEvents { get; set; }
        public bool QrInStudentEmail { get; set; }
        public int MaxLimitStudentSeats { get; set; } = 160;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}