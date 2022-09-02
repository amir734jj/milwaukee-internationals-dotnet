using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
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
        public IActionResult Index()
        {
            var result = _configLogic.ResolveGlobalConfig();
            
            return View(result);
        }
        
        [HttpPost]
        [Route("")]
        [AuthorizeMiddleware(UserRoleEnum.Admin)]
        public async Task<IActionResult> UpdateConfig(GlobalConfigViewModel globalConfigViewModel)
        {
            await _configLogic.SetGlobalConfig(globalConfigViewModel);

            return RedirectToAction("Index");
        }
    }
}