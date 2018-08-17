using System.Linq;
using DAL.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models;
using static Logic.Utilities.DisplayIdUtility;

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

        /// <summary>
        /// Make sure display ID is not null or empty
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override Driver Save(Driver instance)
        {
            // TODO: make this faster
            instance.DisplayId = GenerateDisplayId(instance, _driverDal.GetAll().Select(x => x.Id).Max() + 1);
            
            return base.Save(instance);
        }
    }
}