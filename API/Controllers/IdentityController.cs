using System;
using System.Linq;
using System.Threading.Tasks;
using API.Abstracts;
using API.Attributes;
using DAL.Interfaces;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.ViewModels.Identities;
using reCAPTCHA.AspNetCore;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    public class IdentityController : AbstractAccountController
    {
        private readonly IUserLogic _userLogic;
        private readonly UserManager<User> _userManager;

        private readonly SignInManager<User> _signInManager;

        private readonly RoleManager<IdentityRole<int>> _roleManager;

        private readonly IRecaptchaService _recaptcha;
        private readonly IApiEventService _apiEventService;

        private readonly ILogger<IdentityController> _logger;

        public IdentityController(IUserLogic userLogic, UserManager<User> userManager,
            SignInManager<User> signInManager, RoleManager<IdentityRole<int>> roleManager, IRecaptchaService recaptcha, IApiEventService apiEventService,
            ILogger<IdentityController> logger)
        {
            _userLogic = userLogic;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _recaptcha = recaptcha;
            _apiEventService = apiEventService;
            _logger = logger;
        }

        public override UserManager<User> ResolveUserManager()
        {
            return _userManager;
        }

        public override SignInManager<User> ResolveSignInManager()
        {
            return _signInManager;
        }

        public override RoleManager<IdentityRole<int>> ResolveRoleManager()
        {
            return _roleManager;
        }

        /// <summary>
        ///     View page to login
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Login")]
        [SwaggerOperation("Login")]
        [DisallowAuthenticated]
        public IActionResult Login()
        {
            if (TempData.ContainsKey("Error"))
            {
                var prevError = (string)TempData["Error"];

                ModelState.AddModelError("", prevError);

                TempData.Clear();
            }

            return View();
        }

        /// <summary>
        ///     Handles login the user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("LoginHandler")]
        [SwaggerOperation("LoginHandler")]
        [DisallowAuthenticated]
        public async Task<IActionResult> LoginHandler(LoginViewModel loginViewModel)
        {
            TempData.Clear();

            var recaptcha = await _recaptcha.Validate(Request);

            /*if (!recaptcha.success)
            {
                _logger.LogError("Captcha failed: " + recaptcha.score);

                TempData["Error"] = "There was an error validating recatpcha. Please try again!";

                return RedirectToAction("Login");
            }*/

            var result = await base.Login(loginViewModel);
            
            if (result)
            {
                await _apiEventService.RecordEvent($"User [{loginViewModel.Username}] logged in successfully");

                var user = (await _userLogic.GetAll()).First(x =>
                    x.UserName.Equals(loginViewModel.Username, StringComparison.OrdinalIgnoreCase));

                user.LastLoggedInDate = DateTimeOffset.Now;

                await _userLogic.Update(user.Id, user);

                return RedirectToAction("Index", "Home");
            }

            await _apiEventService.RecordEvent($"User [{loginViewModel.Username}] failed to login");

            return RedirectToAction("NotAuthenticated");
        }

        /// <summary>
        ///     View page to register
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Register")]
        [SwaggerOperation("Register")]
        [DisallowAuthenticated]
        public IActionResult Register()
        {
            if (TempData.ContainsKey("Error"))
            {
                var prevError = (string)TempData["Error"];

                ModelState.AddModelError("", prevError);

                TempData.Clear();
            }

            return View();
        }

        /// <summary>
        ///     Register the user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RegisterHandler")]
        [SwaggerOperation("RegisterHandler")]
        [DisallowAuthenticated]
        public async Task<IActionResult> RegisterHandler(RegisterViewModel registerViewModel)
        {
            TempData.Clear();

            var recaptcha = await _recaptcha.Validate(Request);

            /*if (!recaptcha.success)
            {
                _logger.LogError("Captcha failed: " + recaptcha.score);

                TempData["Error"] = "There was an error validating recaptcha. Please try again!";

                return RedirectToAction("Register");
            }*/

            if (registerViewModel.Password != registerViewModel.ConfirmPassword)
            {
                TempData["Error"] = "Password and Password Confirmation do not match. Please try again!";

                return RedirectToAction("Register");
            }

            // Save the user
            var result = await Register(registerViewModel);

            if (result)
            {
                await _apiEventService.RecordEvent($"User [{registerViewModel.Username}] successfully register");

                return RedirectToAction("Login");
            }
            
            await _apiEventService.RecordEvent($"User [{registerViewModel.Username}] failed to register");

            return RedirectToAction("Register");
        }

        /// <summary>
        ///     Not authenticated view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("NotAuthenticated")]
        [SwaggerOperation("NotAuthenticated")]
        public IActionResult NotAuthenticated()
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
        [Authorize]
        public async Task<IActionResult> LogoutHandler()
        {
            var result = await ResolveUserManager().FindByNameAsync(User.Identity!.Name);
            
            await Logout();
            
            await _apiEventService.RecordEvent($"User [{result.UserName}] successfully logged-out");

            return RedirectToAction("Login");
        }
    }
}