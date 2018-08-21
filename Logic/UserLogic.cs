using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore.Internal;
using Models;
using static Logic.Utilities.HashingUtility;

namespace Logic
{
    public class UserLogic  : BasicCrudLogicAbstract<User>, IUserLogic
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
        protected override IBasicCrudDal<User> GetBasicCrudDal() => _userDal;

        /// <summary>
        /// Override
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override async Task<User> Save(User instance)
        {
            // Make sure username is not duplicate
            if ((await GetAll()).Any(x => x.Username == instance.Username))
            {
                return null;
            }
            
            // Do not store the plain-text password
            instance.Password = SecureHashPassword(instance.Password);
            
            return await base.Save(instance);
        }
    }
}