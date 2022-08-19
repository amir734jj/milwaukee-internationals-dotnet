using System;

namespace Logic.Extensions;

public static class DateTimeOffsetExtension
{
    public static DateTimeOffset ToCentralTime(this DateTimeOffset dateTimeOffset)
    {
        var centralTime = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

        return TimeZoneInfo.ConvertTime(dateTimeOffset, centralTime);
    }
}