using System.Threading.Tasks;
using DAL.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Entities;
using Models.Enums;

namespace Logic
{
    public class UserLogic : BasicCrudLogicAbstract<User>, IUserLogic
    {
        private readonly IUserDal _userDal;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="userDal"></param>
        public UserLogic(IUserDal userDal)
        {
            _userDal = userDal;
        }

        /// <summary>
        /// Returns instance of user DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudDal<User> GetBasicCrudDal()
        {
            return _userDal;
        }
        
        /// <summary>
        /// Updates user role
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userRoleEnum"></param>
        /// <returns></returns>
        public async Task<User> UpdateUserRole(int id, UserRoleEnum userRoleEnum)
        {
            // Update UserRole
            return await _userDal.Update(id, user => user.UserRoleEnum = userRoleEnum);
        }
    }
}