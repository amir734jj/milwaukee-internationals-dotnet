using AutoMapper;

namespace Models.Profiles
{    
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, Student>().ForMember(x => x.Id, opt => opt.Ignore());;
        }
    }
}