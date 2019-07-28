using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

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
        /// Returns registration page for drivers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        /// <summary>
        /// Returns registration page for drivers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Driver")]
        public async Task<IActionResult> Driver()
        {
            return View(new Driver());
        }
        
        /// <summary>
        /// POST registration
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Driver/Register")]
        public async Task<IActionResult> RegisterDriver(Driver driver)
        {
            if (_registrationLogic.RegisterDriver(driver))
            {
                return View("Thankyou");   
            }

            // TODO: use a proper 500 error page
            return Ok("Failed!");
        }

        [HttpGet]
        [Route("Student")]
        public async Task<IActionResult> Student()
        {
            return View(new Student());
        }
        
        /// <summary>
        /// POST registration
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Student/Register")]
        public async Task<IActionResult> RegisterStudent(Student student)
        {
            if (_registrationLogic.RegisterStudent(student))
            {
                return View("Thankyou");   
            }

            // TODO: use a proper 500 error page
            return Ok("Failed!");
        }
        
        [AuthorizeMiddleware]
        [HttpGet]
        [Route("Host")]
        public async Task<IActionResult> Host()
        {
            return View(new Host());
        }
        
        /// <summary>
        /// POST registration
        /// </summary>
        /// <returns></returns>
        [AuthorizeMiddleware]
        [HttpPost]
        [Route("Host/Register")]
        public async Task<IActionResult> RegisterHost(Host host)
        {
            if (_registrationLogic.RegisterHost(host))
            {
                return View("Thankyou");   
            }

            // TODO: use a proper 500 error page
            return Ok("Failed!");
        }
    }
}