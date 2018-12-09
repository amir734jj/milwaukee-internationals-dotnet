using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Models.Entities;

namespace Models.Profiles
{    
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, User>()
                .EqualityComparison((x, y) => x.Id == y.Id);
        }
    }
}