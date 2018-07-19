using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers
{
    [AuthorizeMiddleware]
    [Route("[controller]")]
    public class StudentController : Controller
    {
        private readonly IStudentLogic _studentLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        public StudentController(IStudentLogic studentLogic)
        {
            _studentLogic = studentLogic;
        }
        
        /// <summary>
        /// Returns student view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerOperation("Index")]
        public IActionResult Index()
        {
            return View(_studentLogic.GetAll());
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
            _studentLogic.Delete(id);

            return RedirectToAction("Index");
        }
    }
}