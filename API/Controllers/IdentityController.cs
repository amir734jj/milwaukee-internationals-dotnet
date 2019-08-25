using System.Threading.Tasks;
using API.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.ViewModels.Identities;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    public class IdentityController : AbstractAccountController
    {        
        private readonly UserManager<User> _userManager;
        
        private readonly SignInManager<User> _signInManager;

        public IdentityController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public override UserManager<User> ResolveUserManager()
        {
            return _userManager;
        }

        public override SignInManager<User> ResolveSignInManager()
        {
            return _signInManager;
        }

        /// <summary>
        ///     View page to login
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Login")]
        [SwaggerOperation("Login")]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        
        /// <summary>
        ///     Handles login the user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("LoginHandler")]
        [SwaggerOperation("LoginHandler")]
        public async Task<IActionResult> LoginHandler(LoginViewModel loginViewModel)
        {
            var rslt = await base.Login(loginViewModel);

            if (rslt)
            {
                return Redirect(Url.Content("~/"));
            }

            return RedirectToAction("NotAuthenticated");
        }
        
        /// <summary>
        ///     View page to register
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Register")]
        [SwaggerOperation("Register")]
        public async Task<IActionResult> Register()
        {
            return View();
        }
        
        /// <summary>
        ///     Register the user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RegisterHandler")]
        [SwaggerOperation("RegisterHandler")]
        public async Task<IActionResult> RegisterHandler(RegisterViewModel registerViewModel)
        {
            // Save the user
            var rslt = await Register(registerViewModel);

            if (rslt.Succeeded)
            {
                return RedirectToAction("Login");
            }

            return RedirectToAction("Register");
        }
        
        /// <summary>
        ///     Not authenticated view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("NotAuthenticated")]
        [SwaggerOperation("NotAuthenticated")]
        public async Task<IActionResult> NotAuthenticated()
        {
            return View();
        }

        /// <summary>
        ///     Not authorized view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Logout")]
        [SwaggerOperation("Logout")]
        public async Task<IActionResult> Logout()
        {
            await base.Logout();

            return RedirectToAction("Login");
        }
    }
}