using EntityUpdater.Abstracts;
using Models.Entities;

namespace Models.Profiles
{    
    public class HostProfile : AssignmentProfile<Host>
    {
        public HostProfile()
        {
            Map(x => x.Email)
                .Then(x => x.Phone)
                .Then(x => x.Address)
                .Then(x => x.Fullname);
        }
    }
}