using System;
using Models.ViewModels.Config;
using Newtonsoft.Json;

namespace Models.Constants
{
    public class GlobalConfigs
    {
        public DateTimeOffset LastModified { get; set; }

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
        }

        public bool DisallowDuplicateStudents { get; set; }
        public bool RecordApiEvents { get; set; }

        public object ToAnonymousObject()
        {
            return new
            {
                EventFeature,
                YearValue,
                EmailTestMode,
                CurrentTheme,
                DisallowDuplicateStudents,
                RecordApiEvents
            };
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(ToAnonymousObject());
        }
    }
}