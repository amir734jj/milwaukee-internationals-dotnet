using System;
using System.Linq;
using System.Threading.Tasks;
using API.Attributes;
using Logic.Extensions;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.ViewModels;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    public class UtilityController : Controller
    {
        private readonly IEmailUtilityLogic _emailUtilityLogic;
        
        private readonly IStudentLogic _studentLogic;
        
        private readonly IDriverLogic _driverLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="emailUtilityLogic"></param>
        /// <param name="studentLogic"></param>
        /// <param name="driverLogic"></param>
        public UtilityController(IEmailUtilityLogic emailUtilityLogic, IStudentLogic studentLogic, IDriverLogic driverLogic)
        {
            _emailUtilityLogic = emailUtilityLogic;
            _studentLogic = studentLogic;
            _driverLogic = driverLogic;
        }

        /// <summary>
        /// Returns email utility view
        /// </summary>
        /// <returns></returns>
        [AuthorizeMiddleware]
        [HttpGet]
        [Route("AdHocEmail")]
        [SwaggerOperation("AdHocEmail")]
        public async Task<IActionResult> AdHocEmail(bool status = false)
        {
            return View(new EmailFormViewModel { Status = status });
        }

        /// <summary>
        /// Post action handler
        /// </summary>
        /// <returns></returns>
        [AuthorizeMiddleware]
        [HttpPost]
        [Route("AdHocEmailAction")]
        [SwaggerOperation("AdHocEmailAction")]
        public async Task<IActionResult> AdHocEmailAction(EmailFormViewModel emailFormViewModel)
        {
            // Handle the action
            await _emailUtilityLogic.HandleAdHocEmail(emailFormViewModel);

            return RedirectToAction("AdHocEmail", new { status = true });
        }
        
        /// <summary>
        /// Post action handler
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("EmailCheckIn/{type}/{hashcode}")]
        [SwaggerOperation("EmailCheckIn")]
        public async Task<IActionResult> EmailCheckin([FromRoute] EntitiesEnum type, [FromRoute] int hashcode)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (type)
            {
                case EntitiesEnum.Student when _studentLogic.GetByHashcode(hashcode) != null:
                    return View(_studentLogic.GetByHashcode(hashcode));
                case EntitiesEnum.Driver when _driverLogic.GetByHashcode(hashcode) != null:
                    return View(_driverLogic.GetByHashcode(hashcode));
                default:
                    return Redirect("~/");
            }
        }
        
        /// <summary>
        /// Post action handler
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("EmailCheckInAction/{type}/{id}")]
        [SwaggerOperation("EmailCheckInAction")]
        public async Task<IActionResult> EmailCheckinHandler([FromRoute] EntitiesEnum type, [FromRoute] int id,
            [FromQuery] bool present)
        {
            var result = _emailUtilityLogic.HandleEmailCheckIn(type, id, present);

            // Redirect to home page
            return Ok(result);
        }
    }
}