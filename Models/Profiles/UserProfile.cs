using AutoMapper;

namespace Models.Profiles
{    
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, User>().ForMember(x => x.Id, opt => opt.Ignore());;
        }
    }
}