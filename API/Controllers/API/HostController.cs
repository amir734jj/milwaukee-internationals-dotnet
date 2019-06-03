using API.Abstracts;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Entities;

namespace API.Controllers.Api
{
    [AuthorizeMiddleware]
    [Route("api/[controller]")]
    public class HostController : BasicCrudController<Host>
    {
        private readonly IHostLogic _hostLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="hostLogic"></param>
        public HostController(IHostLogic hostLogic)
        {
            _hostLogic = hostLogic;
        }

        /// <summary>
        /// Returns instance of logic
        /// </summary>
        /// <returns></returns>
        public override IBasicCrudLogic<Host> BasicCrudLogic() => _hostLogic;
    }
}