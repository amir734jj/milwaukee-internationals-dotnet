using System.Linq;
using EfCoreRepository.Interfaces;
using Models.Entities;

namespace DAL.Profiles
{
    public class UserProfile : IEntityProfile<User, int>
    {
        public User Update(User entity, User dto)
        {
            entity.Fullname = dto.Fullname;
            entity.PasswordHash = dto.PasswordHash;
            entity.UserName = dto.UserName;
            entity.UserRoleEnum = dto.UserRoleEnum;
            entity.PhoneNumber = dto.PhoneNumber;
            entity.Email = dto.Email;

            return entity;
        }

        public IQueryable<User> Include<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<User>
        {
            return queryable
                .OrderBy(x => x.Fullname);
        }
    }
}