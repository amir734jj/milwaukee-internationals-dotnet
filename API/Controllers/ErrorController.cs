using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class ErrorController : Controller
    {
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(@"

Something went wrong!                          

                  
 ,adPPYba,   ,adPPYba,  8b,dPPYba,  ,adPPYba,  
a8""     ""8a a8""     ""8a 88P'    ""8a I8[    """"  
8b       d8 8b       d8 88       d8  `""Y8ba,   
""8a,   ,a8"" ""8a,   ,a8"" 88b,   ,a8"" aa    ]8I  
 `""YbbdP""'   `""YbbdP""'  88`YbbdP""'  `""YbbdP""'  
                        88                     
                        88                     

");
        }
    }
}