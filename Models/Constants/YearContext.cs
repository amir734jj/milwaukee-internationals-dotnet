using System;

namespace Models.Constants
{
    public static class YearContext
    {
        public static int YearValue { get; set; } = DateTime.Now.Year;
    }
}