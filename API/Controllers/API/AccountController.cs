using System;
using System.Linq;
using System.Threading.Tasks;
using API.Abstracts;
using API.Utilities;
using DAL.Interfaces;
using Logic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.Identities;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Models.Entities;
namespace Api.Controllers.Api
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AccountController : AbstractAccountController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IUserLogic _userLogic;
        private readonly IApiEventService _apiEventService;

        public AccountController(JwtSettings jwtSettings, UserManager<User> userManager,
            SignInManager<User> signManager, RoleManager<IdentityRole<int>> roleManager, IUserLogic userLogic, IApiEventService apiEventService)
        {
            _jwtSettings = jwtSettings;
            _userManager = userManager;
            _signManager = signManager;
            _roleManager = roleManager;
            _userLogic = userLogic;
            _apiEventService = apiEventService;
        }

        [HttpGet]
        [Route("")]
        [SwaggerOperation("AccountInfo")]
        public async Task<IActionResult> Index()
        {
            if (User.Identity is { IsAuthenticated: true })
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                
                return Ok(user);
            }

            return Ok(new { });
        }

        [NonAction]
        protected override UserManager<User> ResolveUserManager()
        {
            return _userManager;
        }

        [NonAction]
        protected override SignInManager<User> ResolveSignInManager()
        {
            return _signManager;
        }

        [NonAction]
        protected override RoleManager<IdentityRole<int>> ResolveRoleManager()
        {
            return _roleManager;
        }

        [NonAction]
        protected override JwtSettings ResolveJwtSettings()
        {
            return _jwtSettings;
        }

        [HttpPost]
        [Route("Register")]
        [SwaggerOperation("Register")]
        public new async Task<IActionResult> Register([FromBody] RegisterViewModel registerViewModel)
        {
            TempData.Clear();

            if (registerViewModel.Password != registerViewModel.ConfirmPassword)
            {
                TempData["Error"] = "Password and Password Confirmation do not match. Please try again!";

                return BadRequest("Password and Password Confirmation do not match");
            }

            // Save the user
            var (result, errors) = await base.Register(registerViewModel);

            if (result)
            {
                await _apiEventService.RecordEvent($"User [{registerViewModel.Username}] successfully register");

                return Ok("Successfully registered");
            }
            
            await _apiEventService.RecordEvent($"User [{registerViewModel.Username}] failed to register");

            return BadRequest("Failed to register");
        }

        [HttpPost]
        [Route("Login")]
        [SwaggerOperation("Login")]
        public new async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            TempData.Clear();
            
            var (result, message) = await base.Login(loginViewModel);
            
            if (result)
            {
                await _apiEventService.RecordEvent($"User [{loginViewModel.Username}] logged in successfully");

                var user = (await _userLogic.GetAll()).First(x =>
                    x.UserName.Equals(loginViewModel.Username, StringComparison.OrdinalIgnoreCase));

                user.LastLoggedInDate = DateTimeOffset.Now;

                await _userLogic.Update(user.Id, user);

                var token = ResolveToken(user);

                return Ok(new
                {
                    token,
                    user.UserName,
                    user.Email
                });
            }

            await _apiEventService.RecordEvent($"User [{loginViewModel.Username}] failed to login because of {message}");

            return Unauthorized();
        }

        [Authorize]
        [HttpPost]
        [Route("Logout")]
        [SwaggerOperation("Logout")]
        public new async Task<IActionResult> Logout()
        {
            var result = await ResolveUserManager().FindByNameAsync(User.Identity!.Name);
            
            await base.Logout();
            
            await _apiEventService.RecordEvent($"User [{result.UserName}] successfully logged-out");

            return Ok("Successfully logged out");
        }
        
        [Authorize]
        [HttpGet]
        [Route("Refresh")]
        [SwaggerOperation("Refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (User.Identity != null)
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                
                var token = base.ResolveToken(user);

                return Ok(new
                {
                    token,
                    user.UserName,
                    user.Email
                });
            }

            return BadRequest("Failed to find the user");
        }
    }
}