
using EntityUpdater.Abstracts;
using Models.Entities;

namespace Models.Profiles
{    
    public class UserProfile : AssignmentProfile<User>
    {
        public UserProfile()
        {
            Map(x => x.Email)
                .Then(x => x.Phone)
                .Then(x => x.Fullname)
                .Then(x => x.Password)
                .Then(x => x.UserRoleEnum)
                .Then(x => x.Username);
        }
    }
}