using System;
using System.Threading.Tasks;
using API.Attributes;
using API.Extensions;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace API.Controllers
{
    [AllowAnonymous]
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
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Returns registration pages
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Driver")]
        public IActionResult Driver()
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewData["Error"] = TempData["Error"];
                TempData.Clear();
            }
            
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
            try
            {
                await _registrationLogic.RegisterDriver(driver);

                ModelState.ClearModelStateErrors();

                return View("Thankyou");
            }
            catch (Exception e)
            {
                TempData["Error"] = $"Failed to register driver. Please try again! {e.Message}";

                return RedirectToAction("Driver");
            }
        }

        [HttpGet]
        [Route("Student")]
        public IActionResult Student()
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewData["Error"] = TempData["Error"];
                TempData.Clear();
            }
            
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
            try
            {
                await _registrationLogic.RegisterStudent(student);

                ModelState.ClearModelStateErrors();

                return View("Thankyou");
            }
            catch (Exception e)
            {
                TempData["Error"] = $"Failed to register student. Please try again! {e.Message}";

                return RedirectToAction("Student");
            }
        }
        
        [AuthorizeMiddleware]
        [HttpGet]
        [Route("Host")]
        public IActionResult Host()
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewData["Error"] = TempData["Error"];
                TempData.Clear();
            }
            
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
            try
            {
                await _registrationLogic.RegisterHost(host);

                ModelState.ClearModelStateErrors();

                return View("Thankyou");
            }
            catch (Exception e)
            {
                TempData["Error"] = $"Failed to register host. Please try again! {e.Message}";

                return RedirectToAction("Host");
            }
        }
        
        [AuthorizeMiddleware]
        [HttpGet]
        [Route("Event")]
        public IActionResult Event()
        {
            return View(new Event());
        }
        
        /// <summary>
        /// POST registration
        /// </summary>
        /// <returns></returns>
        [AuthorizeMiddleware]
        [HttpPost]
        [Route("Event/Register")]
        public async Task<IActionResult> RegisterEvent(Event @event)
        {
            try
            {
                await _registrationLogic.RegisterEvent(@event);

                ModelState.ClearModelStateErrors();

                return View("Thankyou");
            }
            catch (Exception e)
            {
                TempData["Error"] = $"Failed to register event. Please try again! {e.Message}";

                return RedirectToAction("Event");
            }
        }
    }
}