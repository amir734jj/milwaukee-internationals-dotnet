using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.ViewModels;
using Models.ViewModels.Config;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    [AuthorizeMiddleware]
    public class ConfigController : Controller
    {
        private readonly IConfigLogic _configLogic;

        public ConfigController(IConfigLogic configLogic)
        {
            _configLogic = configLogic;
        }
        
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var result = await _configLogic.ResolveYearContext();
            
            return View("ChangeYearContext", result);
        }
        
        [HttpPost]
        [Route("")]
        [UserRoleMiddleware(UserRoleEnum.Admin)]
        public async Task<IActionResult> UpdateYearContext(GlobalConfigViewModel globalConfigViewModel)
        {
            await _configLogic.SetYearContext(globalConfigViewModel.UpdatedYear);

            return RedirectToAction("Index");
        }
    }
}