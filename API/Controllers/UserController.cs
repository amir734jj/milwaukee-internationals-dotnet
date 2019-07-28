using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AuthorizeMiddleware]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserLogic _userLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="userLogic"></param>
        public UserController(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }

        /// <summary>
        /// Returns user view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            return View(await _userLogic.GetAll());
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userLogic.Delete(id);

            return RedirectToAction("Index");
        }
         
        /// <summary>
        /// Update User Role
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("UpdateUserRole/{id}/{userRoleEnum}")]
        public async Task<IActionResult> UpdateUserRole(int id, UserRoleEnum userRoleEnum)
        {
            await _userLogic.UpdateUserRole(id, userRoleEnum);

            return RedirectToAction("Index");
        }
    }
}