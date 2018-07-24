using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        /// Returns driver view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerOperation("Index")]
        public IActionResult Index()
        {
            return View(_userLogic.GetAll());
        }

        /// <summary>
        /// Delete a driver
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Delete/{id}")]
        [SwaggerOperation("Delete")]
        public IActionResult Delete(int id)
        {
            _userLogic.Delete(id);

            return RedirectToAction("Index");
        }
    }
}