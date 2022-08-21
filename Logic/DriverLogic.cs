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
using static Logic.Utilities.RegistrationUtility;

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

            // Normalize phone number
            instance.Phone = NormalizePhoneNumber(instance.Phone);
            
            var lastDisplayId = (await base.GetAll(DateTime.UtcNow.Year)).MaxBy(x => x.Id)
                ?.DisplayId;
            var lastId = lastDisplayId != null ? int.Parse(lastDisplayId.Split("-")[1]) : 0;

            // Set the year
            instance.Year = DateTime.UtcNow.Year;

            instance.DisplayId = GenerateDisplayId(instance, lastId);

            // Save the instance
            var retVal = await base.Save(instance);

            return retVal;
        }

        public override async Task<Driver> Update(int id, Driver driver)
        {
            // If role is navigator then capacity is 0
            if (driver.Role == RolesEnum.Navigator)
            {
                driver.Capacity = 0;
            }

            // Update only subset of properties
            return await base.Update(id, x =>
            {
                x.DisplayId = driver.DisplayId;
                x.Fullname = driver.Fullname;
                x.Email = driver.Email;
                x.Phone = driver.Phone;
                x.Role = driver.Role;
                x.Capacity = driver.Capacity;
                x.HaveChildSeat = driver.HaveChildSeat;
                x.RequireNavigator = driver.RequireNavigator;
                x.Navigator = driver.Navigator;
            });
        }

        protected override IBasicCrud<Driver> Repository()
        {
            return _dal;
        }

        public override async Task<IEnumerable<Driver>> GetAll(string sortBy = null, bool? descending = null)
        {
            return (await base.GetAll(sortBy, descending)).Where(x => x.Year == _globalConfigs.YearValue);
        }
    }
}