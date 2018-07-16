using DAL.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
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
        public override IBasicCrudDal<User> GetBasicCrudDal() => _userDal;

        /// <summary>
        /// Override
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override User Save(User instance)
        {
            // Do not store the plain-text password
            instance.Password = SecureHashPassword(instance.Password);
            
            return base.Save(instance);
        }
    }
}