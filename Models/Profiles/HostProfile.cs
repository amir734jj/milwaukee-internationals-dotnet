using AutoMapper;

namespace Models.Profiles
{    
    public class HostProfile : Profile
    {
        public HostProfile()
        {
            CreateMap<Host, Host>().ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}