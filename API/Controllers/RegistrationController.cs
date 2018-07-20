using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    public class RegistrationController : Controller
    {
        private readonly IRegistrationLogic _registrationLogic;

        public RegistrationController(IRegistrationLogic registrationLogic)
        {
            _registrationLogic = registrationLogic;
        }
        
        /// <summary>
        /// Returns registeration page for drivers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
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
            if (_registrationLogic.RegisterDriver(driver))
            {
                return View("Thankyou");   
            }

            // TODO: use a proper 500 error page
            return Ok("Failed!");
        }

        [HttpGet]
        [Route("student")]
        public IActionResult Student()
        {
            return View(new Student());
        }
        
        /// <summary>
        /// POST registertaion
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("student/register")]
        public IActionResult RegisterStudent(Student student)
        {
            if (_registrationLogic.RegisterStudent(student))
            {
                return View("Thankyou");   
            }

            // TODO: use a proper 500 error page
            return Ok("Failed!");
        }
    }
}