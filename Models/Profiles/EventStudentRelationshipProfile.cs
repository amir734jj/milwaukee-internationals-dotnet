using AutoMapper;
using Models.Entities;

namespace Models.Profiles
{
    public class EventStudentRelationshipProfile : Profile
    {
        public EventStudentRelationshipProfile()
        {
            CreateMap<EventStudentRelationship, EventStudentRelationship>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}