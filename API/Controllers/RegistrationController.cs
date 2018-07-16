using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Route("[controller]")]
    public class RegistrationController : Controller
    {
        private readonly IDriverLogic _driverLogic;

        public RegistrationController(IDriverLogic driverLogic)
        {
            _driverLogic = driverLogic;
        }

        /// <summary>
        /// Returns registeration page for drivers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("driver")]
        public IActionResult Driver()
        {
            return View(new Driver());
        }
        
        /// <summary>
        /// POST registertaion
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("driver/register")]
        public IActionResult RegisterDriver(Driver driver)
        {
            // Save the driver
            _driverLogic.Save(driver);
            
            return Redirect("~/");
        }

        [HttpGet]
        [Route("student")]
        public IActionResult Student()
        {
            return View(new Student());
        }
    }
}