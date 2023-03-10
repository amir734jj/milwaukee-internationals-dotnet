using System;
using AutoMapper;
using Models.Constants;
using Models.ViewModels.Config;

namespace Models.Profiles;

public class DateTimeWithZoneTypeConverter : IValueConverter<DateTimeOffset, DateTime>
{
    public DateTime Convert(DateTimeOffset source, ResolutionContext context)
    {
        return source.DateTime;
    }
}

public class DateTimeWithoutZoneTypeConverter : IValueConverter<DateTime, DateTimeOffset>
{
    public DateTimeOffset Convert(DateTime source, ResolutionContext context)
    {
        var tzi = ApplicationConstants.TimeZoneInfo;
        
        return TimeZoneInfo.ConvertTime(source, tzi);
    }
}

public class GlobalConfigsMappingProfile: Profile
{
    public GlobalConfigsMappingProfile()
    {
        CreateMap<GlobalConfigs, GlobalConfigViewModel>()
            .ForMember(x => x.TourDate, x => x.ConvertUsing(new DateTimeWithZoneTypeConverter()));
        CreateMap<GlobalConfigViewModel, GlobalConfigs>()
            .ForMember(x => x.TourDate, x => x.ConvertUsing(new DateTimeWithoutZoneTypeConverter()));
    }
}