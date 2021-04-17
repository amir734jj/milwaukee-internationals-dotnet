using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Models.Entities;
using Models.Enums;

namespace Logic
{
    public class UserLogic : BasicCrudLogicAbstract<User>, IUserLogic
    {
        private readonly IBasicCrud<User> _userDal;

        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="userManager"></param>
        public UserLogic(IEfRepository repository, UserManager<User> userManager)
        {
            _userDal = repository.For<User>();
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

        /// <inheritdoc />
        /// <summary>
        /// Returns instance of user DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrud<User> GetBasicCrudDal()
        {
            return _userDal;
        }
    }
}