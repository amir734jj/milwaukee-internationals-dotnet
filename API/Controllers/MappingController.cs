using System.Linq;
using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AuthorizeMiddleware]
    [Route("[controller]")]
    public class MappingController : Controller
    {
        private readonly IStudentLogic _studentLogic;
        private readonly IDriverLogic _driverLogic;

        public MappingController(IStudentLogic studentLogic, IDriverLogic driverLogic)
        {
            _studentLogic = studentLogic;
            _driverLogic = driverLogic;
        }
        
        // GET the view
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Returns Student-Driver Mapping view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("StudentDriverMapping")]
        public async Task<IActionResult> StudentDriverMapping()
        {
            var students = (await _studentLogic.GetAll()).ToList();
            
            return View(students);
        }

        /// <summary>
        /// Returns Driver-Host Mapping view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("DriverHostMapping")]
        public async Task<IActionResult> DriverHostMapping()
        {
            var drivers = (await _driverLogic.GetAll()).ToList();

            return View(drivers);
        }
    }
}