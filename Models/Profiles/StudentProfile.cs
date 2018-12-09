using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Models.Entities;

namespace Models.Profiles
{    
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, Student>()
                .EqualityComparison((x, y) => x.Id == y.Id);
        }
    }
}