using AutoMapper;
using Models.Entities;

namespace Models.Profiles
{    
    public class DriverProfile : Profile
    {
        public DriverProfile()
        {
            CreateMap<Driver, Driver>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}