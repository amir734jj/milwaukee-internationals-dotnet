using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Models.Entities;

namespace Models.Profiles
{
    public class EventStudentRelationshipProfile : Profile
    {
        public EventStudentRelationshipProfile()
        {
            CreateMap<EventStudentRelationship, EventStudentRelationship>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .EqualityComparison((x, y) => x.Id == y.Id);
        }
    }
}