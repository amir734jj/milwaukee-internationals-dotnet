using AutoMapper;

namespace Models.Profiles
{    
    public class DriverProfile : Profile
    {
        public DriverProfile()
        {
            CreateMap<Driver, Driver>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.HostRefId, opt => opt.MapFrom(x => x.Host.Id));
        }
    }
}