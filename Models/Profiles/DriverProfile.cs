using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Models.Entities;

namespace Models.Profiles
{    
    public class DriverProfile : Profile
    {
        public DriverProfile()
        {
            CreateMap<Driver, Driver>()
                .EqualityComparison((x, y) => x.Id == y.Id);
        }
    }
}