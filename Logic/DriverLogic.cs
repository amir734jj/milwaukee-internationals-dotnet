using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Constants;
using Models.Entities;
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
        protected override IBasicCrudDal<Driver> GetBasicCrudDal()
        {
            return _driverDal;
        }

        /// <summary>
        /// Make sure display ID is not null or empty
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override async Task<Driver> Save(Driver instance)
        {
            // Set the display id
            instance.DisplayId = GenerateDisplayId(instance, instance.Id);
            
            // Set the year
            instance.Year = DateTime.Now.Year;

            // Save the instance
            var retVal = await base.Save(instance);
            
            return retVal;
        }

        public override async Task<IEnumerable<Driver>> GetAll()
        {
            return (await base.GetAll()).Where(x => x.Year == YearContext.YearValue);
        }
    }
}