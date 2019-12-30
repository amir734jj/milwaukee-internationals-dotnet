using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.ViewModels;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AuthorizeMiddleware]
    [Route("[controller]")]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;

        private readonly IProfileLogic _profileLogic;

        public ProfileController(UserManager<User> userManager, IProfileLogic profileLogic)
        {
            _userManager = userManager;
            _profileLogic = profileLogic;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(_profileLogic.ResolveProfile(await _userManager.FindByNameAsync(User.Identity.Name)));
            }

            return new RedirectResult("~/");
        }
        
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Update(ProfileViewModel profileViewModel)
        {
            await _profileLogic.UpdateUser(profileViewModel);

            return new RedirectResult("~/");
        }
    }
}