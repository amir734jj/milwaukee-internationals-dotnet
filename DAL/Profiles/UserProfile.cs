using System.Linq;
using DAL.Interfaces;
using Models.Entities;

namespace DAL.Profiles
{
    public class UserProfile :  IEntityProfile<User>
    {
        public  void Update(User entity, User dto)
        {
            entity.Fullname = dto.Fullname;
            entity.PasswordHash = dto.PasswordHash;
            entity.UserName = dto.UserName;
            entity.UserRoleEnum = dto.UserRoleEnum;
            entity.PhoneNumber = dto.PhoneNumber;
            entity.Email = dto.Email;
        }

        public IQueryable<User> Include(IQueryable<User> queryable)

        {
            return queryable
                .OrderBy(x => x.Fullname);
        }
    }
}