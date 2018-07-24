using System.Collections.Generic;

namespace Models.ViewModels
{
    /// <summary>
    /// Driver-Host mapping all
    /// </summary>
    public class DriverHostMappingViewModel
    {
        public IEnumerable<Driver> AvailableDrivers { get; set; }
        
        public IEnumerable<Host> AvailableHosts { get; set; }
        
        public IEnumerable<Driver> MappedDrivers { get; set; }
        
        public IEnumerable<Host> MappedHosts { get; set; }
    }
}