using DAL.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models;

namespace Logic
{
    public class DriverLogic : BasicCrudLogicAbstract<Driver>, IDriverLogic
    {
        private readonly IDriverDal _driverDal;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="driverDal"></param>
        public DriverLogic(IDriverDal driverDal)
        {
            _driverDal = driverDal;
        }

        /// <summary>
        /// Returns instance of driver DAL
        /// </summary>
        /// <returns></returns>
        public override IBasicCrudDal<Driver> GetBasicCrudDal() => _driverDal;
    }
}