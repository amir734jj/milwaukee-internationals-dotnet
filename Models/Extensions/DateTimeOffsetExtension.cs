using System;
using Models.Constants;

namespace Models.Extensions;

public static class DateTimeOffsetExtension
{
    public static DateTimeOffset ToCentralTime(this DateTimeOffset dateTimeOffset)
    {
        var tzi = ApplicationConstants.TimeZoneInfo;
        
        return TimeZoneInfo.ConvertTime(dateTimeOffset, tzi);
    }
}