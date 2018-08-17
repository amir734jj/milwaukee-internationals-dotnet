using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AuthorizeMiddleware]
    [Route("[controller]")]
    public class UtilityController : Controller
    {
        private readonly IEmailUtilityLogic _emailUtilityLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="emailUtilityLogic"></param>
        public UtilityController(IEmailUtilityLogic emailUtilityLogic)
        {
            _emailUtilityLogic = emailUtilityLogic;
        }

        /// <summary>
        /// Returns email utility view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Email")]
        [SwaggerOperation("Email")]
        public async Task<IActionResult> Email(bool status = false)
        {
            return View(new EmailFormViewModel { Status = status });
        }

        /// <summary>
        /// Post action handler
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("EmailAction")]
        [SwaggerOperation("EmailAction")]
        public async Task<IActionResult> EmailAction(EmailFormViewModel emailFormViewModel)
        {
            // Handle the action
            await _emailUtilityLogic.HandleAdHocEmail(emailFormViewModel);

            return RedirectToAction("Email", new { status = true });
        }
    }
}