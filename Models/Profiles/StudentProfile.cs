
using EntityUpdater.Abstracts;
using Models.Entities;

namespace Models.Profiles
{    
    public class StudentProfile : AssignmentProfile<Student>
    {
        public StudentProfile()
        {
            Map(x => x.Email)
                .Then(x => x.Phone)
                .Then(x => x.Fullname)
                .Then(x => x.IsPresent)
                .Then(x => x.Major)
                .Then(x => x.Country)
                .Then(x => x.Interests)
                .Then(x => x.KosherFood)
                .Then(x => x.University)
                .Then(x => x.FamilySize)
                .Then(x => x.NeedCarSeat);
        }
    }
}