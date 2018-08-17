using AutoMapper;

namespace Models.Profiles
{    
    public class DriverProfile : Profile
    {
        public DriverProfile()
        {
            CreateMap<Driver, Driver>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.HostRefId, opt =>
                {
                    // If host is not null
                    opt.Condition(x => x.Host != null);

                    opt.MapFrom(x => x.Host.Id);
                })
                .ForMember(x => x.Host, opt =>
                {
                    // If condition
                    opt.Condition((x, y) => x.HostRefId != x.Host?.Id);
                });
        }
    }
}