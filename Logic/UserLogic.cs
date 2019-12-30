using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using DAL.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Models.Entities;
using Models.Enums;

namespace Logic
{
    public class UserLogic : BasicCrudLogicAbstract<User>, IUserLogic
    {
        private readonly IUserDal _userDal;

        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="userDal"></param>
        /// <param name="userManager"></param>
        public UserLogic(IUserDal userDal, UserManager<User> userManager)
        {
            _userDal = userDal;
            _userManager = userManager;
        }

        public override async Task<User> Get(int id)
        {
            var fetchUserObrv = base.Get(id)
                .ToObservable()
                .Then(user =>
                {
                    var roles = _userManager.GetRolesAsync(user).Result;

                    user.UserRoleEnum = roles.Contains(UserRoleEnum.Admin.ToString())
                        ? UserRoleEnum.Admin
                        : UserRoleEnum.Basic;

                    return user;
                });

            var rslt = await Observable.When(fetchUserObrv);

            return rslt;
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            var fetchUsersObrv = base.GetAll()
                .ToObservable()
                .Then(users => users.Select(user =>
                {
                    var roles = _userManager.GetRolesAsync(user).Result;

                    user.UserRoleEnum = roles.Contains(UserRoleEnum.Admin.ToString())
                        ? UserRoleEnum.Admin
                        : UserRoleEnum.Basic;

                    return user;
                }));

            var rslt = await Observable.When(fetchUsersObrv);

            return rslt;
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns instance of user DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudDal<User> GetBasicCrudDal()
        {
            return _userDal;
        }
    }
}