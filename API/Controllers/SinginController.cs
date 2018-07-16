using API.Extensions;
using Logic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Constants;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class SinginController : Controller
    {
        private readonly ISigninLogic _signinLogic;

        public SinginController(ISigninLogic signinLogic)
        {
            _signinLogic = signinLogic;
        }

        /// <summary>
        /// Login the user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        [SwaggerOperation("Login")]
        public IActionResult Login([FromQuery] string username, [FromQuery] string password)
        {
            _signinLogic.TryLogin(username, password, out var result);

            // Set session values
            if (result)
            {
                HttpContext.Session.SetString(ApiConstants.Username, username);
                HttpContext.Session.SetString(ApiConstants.Password, password);
            }
            
            return result ? (IActionResult) Redirect(Url.Content("~/")) : RedirectToAction("NotAuthenticated");
        }

        /// <summary>
        /// Login the user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Logout")]
        [SwaggerOperation("Logout")]
        public IActionResult Logout()
        {
            var (username, password) = HttpContext.Session.GetUseramePassword();
            
            _signinLogic.TryLogout(username, password, out var result);

            return result ? (IActionResult) Redirect(Url.Content("~/")) : RedirectToAction("NotAuthenticated");
        }
        
        /// <summary>
        /// Not authenticated view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("NotAuthenticated")]
        [SwaggerOperation("NotAuthenticated")]
        public IActionResult NotAuthenticated()
        {
            return View();
        }
    }
}