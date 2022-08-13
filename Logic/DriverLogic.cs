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
        private readonly IBasicCrud<Driver> _dal;
        private readonly GlobalConfigs _globalConfigs;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="globalConfigs"></param>
        public DriverLogic(IEfRepository repository, GlobalConfigs globalConfigs)
        {
            _dal = repository.For<Driver>();
            _globalConfigs = globalConfigs;
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

            var lastDisplayId = (await base.GetAll(DateTime.UtcNow.Year)).OrderByDescending(x =>  x.Id).First().DisplayId;
            var lastId = int.Parse(lastDisplayId.Split("-")[1]);

            // Set the year
            instance.Year = DateTime.UtcNow.Year;

            instance.DisplayId = GenerateDisplayId(instance, lastId);

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

        protected override IBasicCrud<Driver> Repository()
        {
            return _dal;
        }

        public override async Task<IEnumerable<Driver>> GetAll()
        {
            return (await base.GetAll()).Where(x => x.Year == _globalConfigs.YearValue);
        }
    }
}