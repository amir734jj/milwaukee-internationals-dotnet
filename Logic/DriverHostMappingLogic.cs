using System.Collections.Generic;
using System.Linq;
using Logic.Interfaces;
using Models;
using Models.Interfaces;
using Models.ViewModels;

namespace Logic
{
    public class DriverHostMappingLogic : IDriverHostMappingLogic
    {
        private readonly IDriverLogic _driverLogic;
        
        private readonly IHostLogic _hostLogic;

        /// <summary>
        /// Driver-Host mapping logic
        /// </summary>
        /// <param name="driverLogic"></param>
        /// <param name="hostLogic"></param>
        public DriverHostMappingLogic(IDriverLogic driverLogic, IHostLogic hostLogic)
        {
            _driverLogic = driverLogic;
            _hostLogic = hostLogic;
        }

        /// <summary>
        /// Logic to handle the mapping
        /// </summary>
        /// <param name="newDriverHostMappingViewModel"></param>
        /// <returns></returns>
        public bool MapDriverToHost(NewDriverHostMappingViewModel newDriverHostMappingViewModel)
        {
            var driver = _driverLogic.Get(newDriverHostMappingViewModel.DriverId);
            var host = _hostLogic.Get(newDriverHostMappingViewModel.HostId);

            // Initialize the list if it not already initialized
            host.Drivers = host.Drivers ?? new List<Driver>();
            
            // Add the map
            driver.Host = host;

            // Save changes
            _driverLogic.Update(driver.Id, driver);

            return true;
        }

        /// <summary>
        /// Un-Map student from driver
        /// </summary>
        /// <param name="newDriverHostMappingViewModel"></param>
        /// <returns></returns>
        public bool UnMapDriverToHost(NewDriverHostMappingViewModel newDriverHostMappingViewModel)
        {
            var driver = _driverLogic.Get(newDriverHostMappingViewModel.DriverId);
            var host = _hostLogic.Get(newDriverHostMappingViewModel.HostId);

            // Add the map
            driver.Host = null;

            // Save changes
            _driverLogic.Update(driver.Id, driver);

            return true;
        }

        /// <summary>
        /// Returns the status of mappings
        /// </summary>
        /// <returns></returns>
        public DriverHostMappingViewModel MappingStatus()
        {
            var hosts = _hostLogic.GetAll().ToList();
            var drivers = _driverLogic.GetAll().ToList();
            
            // TODO: add check to return only students that are pressent
            return new DriverHostMappingViewModel
            {
                AvailableHosts = hosts,
                AvailableDrivers = drivers.Where(x => x.Host == null),
                MappedDrivers = drivers.Where(x => x.Host != null),
                MappedHosts = hosts.Where(x => x.Drivers != null && x.Drivers.Any())
            };
        }
    }
}