using System.Linq;
using EfCoreRepository;
using Models.Entities;

namespace DAL.Profiles
{
    public class UserProfile :  EntityProfile<User>
    {
        public override void Update(User entity, User dto)
        {
            entity.Fullname = dto.Fullname;
            entity.PasswordHash = dto.PasswordHash;
            entity.UserName = dto.UserName;
            entity.UserRoleEnum = dto.UserRoleEnum;
            entity.PhoneNumber = dto.PhoneNumber;
            entity.Email = dto.Email;
            entity.LastLoggedInDate = dto.LastLoggedInDate;
        }

        public override IQueryable<User> Include<TQueryable>(TQueryable queryable)

        {
            return queryable
                .OrderBy(x => x.Fullname);
        }
    }
}