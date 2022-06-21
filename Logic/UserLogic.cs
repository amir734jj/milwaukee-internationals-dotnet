using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Profiles;
using DAL.Utilities;
using Logic.Abstracts;
using Logic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Models.Entities;
using Models.Enums;

namespace Logic
{
    public class UserLogic : BasicCrudLogicAbstract<User>, IUserLogic
    {
        private readonly EntityDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="userManager"></param>
        public UserLogic(EntityDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public override async Task<User> Get(int id)
        {
            var fetchUserObservable = base.Get(id)
                .ToObservable()
                .Then(user =>
                {
                    var roles = _userManager.GetRolesAsync(user).Result;

                    user.UserRoleEnum = roles.Contains(UserRoleEnum.Admin.ToString())
                        ? UserRoleEnum.Admin
                        : UserRoleEnum.Basic;

                    return user;
                });

            var result = await Observable.When(fetchUserObservable);

            return result;
        }

        protected override EntityDbContext GetDbContext()
        {
            return _dbContext;
        }

        protected override IEntityProfile<User> Profile()
        {
            return new UserProfile();
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            var fetchUsersObservable = base.GetAll()
                .ToObservable()
                .Then(users => users.Select(user =>
                {
                    var roles = _userManager.GetRolesAsync(user).Result;

                    user.UserRoleEnum = roles.Contains(UserRoleEnum.Admin.ToString())
                        ? UserRoleEnum.Admin
                        : UserRoleEnum.Basic;

                    return user;
                }));

            var result = await Observable.When(fetchUsersObservable);

            return result;
        }
    }
}