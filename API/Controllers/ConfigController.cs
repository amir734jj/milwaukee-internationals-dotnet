using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.ViewModels;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
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
        public async Task<IActionResult> UpdateYearContext(YearContextViewModel yearContextViewModel)
        {
            await _configLogic.SetYearContext(yearContextViewModel.UpdatedYear);

            return RedirectToAction("Index");
        }
    }
}