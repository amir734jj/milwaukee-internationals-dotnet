using System.Linq;
using DAL.Interfaces;
using EfCoreRepository;
using Models.Entities;

namespace DAL.Profiles
{
    public class UserProfile :  EntityProfile<User>, IEntityProfile<User> 
    {
        public override void Update(User entity, User dto)
        {
            entity.Fullname = dto.Fullname;
            entity.PasswordHash = dto.PasswordHash;
            entity.UserName = dto.UserName;
            entity.UserRoleEnum = dto.UserRoleEnum;
            entity.PhoneNumber = dto.PhoneNumber;
            entity.Email = dto.Email;
        }

        public override IQueryable<User> Include<TQueryable>(TQueryable queryable)

        {
            return queryable
                .OrderBy(x => x.Fullname);
        }
    }
}