using AutoMapper;

namespace Models.Profiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, Event>().ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}