using AutoMapper;

namespace Models.Profiles
{    
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, Student>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Driver, opt => opt.Ignore())
                .ForMember(x => x.DriverRefId, opt => opt.Ignore())
                .BeforeMap((source, destination) =>
                {
                    destination.DriverRefId = source.DriverRefId;
                    destination.Driver = source.Driver;
                });
        }
    }
}