using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Interfaces;
using Models.ViewModels;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers.Api
{
    [AuthorizeMiddleware]
    [Route("api/[controller]")]
    public class DriverHostMappingController : Controller
    {
        private readonly IDriverHostMappingLogic _driverHostMappingLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="driverHostMappingLogic"></param>
        public DriverHostMappingController(IDriverHostMappingLogic driverHostMappingLogic)
        {
            _driverHostMappingLogic = driverHostMappingLogic;
        }
        
        /// <summary>
        /// Returns the status of mappings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Status")]
        [SwaggerOperation("DriverHostMappingStatus")]
        public IActionResult DriverHostMappingStatus()
        {
            return Ok(_driverHostMappingLogic.MappingStatus());
        }
        
        /// <summary>
        /// Maps the driver to host
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Map")]
        [SwaggerOperation("DriverHostMappingMap")]
        public IActionResult DriverHostMappingMap([FromBody] NewDriverHostMappingViewModel newDriverHostMappingViewModel)
        {
            return Ok(_driverHostMappingLogic.MapDriverToHost(newDriverHostMappingViewModel));
        }
        
        /// <summary>
        /// Un-Maps driver from host
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("UnMap")]
        [SwaggerOperation("DriverHostMappingUnMap")]
        public IActionResult DriverHostMappingUnMap([FromBody] NewDriverHostMappingViewModel newDriverHostMappingViewModel)
        {
            return Ok(_driverHostMappingLogic.UnMapDriverToHost(newDriverHostMappingViewModel));
        }
        
        /// <summary>
        /// Email mappings to hosts
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("EmailMappings")]
        [SwaggerOperation("EmailMappings")]
        public IActionResult EmailMappings()
        {
            return Ok(_driverHostMappingLogic.EmailMappings());
        }
    }
}