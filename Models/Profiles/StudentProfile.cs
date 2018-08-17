using AutoMapper;

namespace Models.Profiles
{    
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, Student>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Driver, opt => opt.MapAtRuntime())
                .ForMember(x => x.DriverRefId, opt =>
                {
                    // If driver is not null
                    opt.Condition(x => x.Driver != null);

                    opt.MapFrom(x => x.Driver.Id);
                });
        }
    }
}