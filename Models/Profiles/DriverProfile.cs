using AutoMapper;

namespace Models.Profiles
{    
    public class DriverProfile : Profile
    {
        public DriverProfile()
        {
            CreateMap<Driver, Driver>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Host, opt => opt.Ignore())
                .ForMember(x => x.HostRefId, opt => opt.Ignore())
                .BeforeMap((source, destination) =>
                {
                    destination.HostRefId = source.HostRefId;
                    destination.Host = source.Host;
                });
        }
    }
}