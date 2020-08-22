using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Constants;
using Models.Entities;
using Models.Enums;
using static Logic.Utilities.DisplayIdUtility;

namespace Logic
{
    public class DriverLogic : BasicCrudLogicAbstract<Driver>, IDriverLogic
    {
        private readonly IBasicCrudType<Driver, int> _driverDal;
        
        private readonly GlobalConfigs _globalConfigs;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="globalConfigs"></param>
        public DriverLogic(IEfRepository repository, GlobalConfigs globalConfigs)
        {
            _driverDal = repository.For<Driver, int>();
            _globalConfigs = globalConfigs;
        }

        /// <summary>
        /// Returns instance of driver DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudType<Driver, int> GetBasicCrudDal()
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
            // If role is navigator then capacity is 0
            if (instance.Role == RolesEnum.Navigator)
            {
                instance.Capacity = 0;
            }
            
            var count = (await base.GetAll(DateTime.UtcNow.Year)).Count();
          
            instance.DisplayId = "Null";

            // Set the year
            instance.Year = DateTime.UtcNow.Year;

            instance.DisplayId = GenerateDisplayId(instance, count);

            // Save the instance
            var retVal = await base.Save(instance);

            return retVal;
        }

        public override async Task<Driver> Update(int id, Driver instance)
        {
            // If role is navigator then capacity is 0
            if (instance.Role == RolesEnum.Navigator)
            {
                instance.Capacity = 0;
            }

            return await base.Update(id, instance);
        }

        public override async Task<IEnumerable<Driver>> GetAll()
        {
            return (await base.GetAll()).Where(x => x.Year == _globalConfigs.YearValue);
        }
    }
}