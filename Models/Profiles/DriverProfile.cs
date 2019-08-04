using EntityUpdater.Abstracts;
using Models.Entities;

namespace Models.Profiles
{
    public class DriverProfile : AssignmentProfile<Driver>
    {
        public DriverProfile()
        {
            Map(x => x.Fullname)
                .Then(x => x.Role)
                .Then(x => x.Phone)
                .Then(x => x.Email)
                .Then(x => x.Capacity)
                .Then(x => x.Navigator)
                .Then(x => x.DisplayId)
                .Then(x => x.RequireNavigator)
                .Then(x => x.HaveChildSeat);
        }
    }
}