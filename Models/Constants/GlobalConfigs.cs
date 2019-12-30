using System;
using Models.ViewModels.Config;

namespace Models.Constants
{
    public static class GlobalConfigs
    {
        public static bool EventFeature { get; private set; }
        
        public static int YearValue { get; private set; } = DateTime.UtcNow.Year;

        public static bool EmailTestMode { get; private set; }

        /// <summary>
        ///  Current website theme
        /// </summary>
        public static string CurrentTheme = "default";

        public static void UpdateGlobalConfigs(GlobalConfigViewModel globalConfigViewModel)
        {
            YearValue = globalConfigViewModel.UpdatedYear;
            EventFeature = globalConfigViewModel.EventFeature;
            EmailTestMode = globalConfigViewModel.EmailTestMode;
            CurrentTheme = globalConfigViewModel.Theme;
        }

        public static object ToAnonymousObject()
        {
            return new
            {
                EventFeature,
                YearValue,
                EmailTestMode,
                CurrentTheme
            };
        }
    }
}