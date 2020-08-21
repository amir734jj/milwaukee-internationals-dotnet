using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;

namespace API.Controllers.AdHoc
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [Route("AdHoc/Registration")]
    public class AdHocRegistrationController : Controller
    {
        [Route("Student")]
        public IActionResult Index()
        {
            return View(new AdHocStudentRegistrationViewModel());
        }
    }
}