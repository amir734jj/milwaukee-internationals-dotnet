using AutoMapper;

namespace Models.Profiles
{    
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, Student>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.DriverRefId, opt =>
                {
                    // If condition
                    opt.Condition((x, y) => x.DriverRefId != x.Driver?.Id);
                });
        }
    }
}