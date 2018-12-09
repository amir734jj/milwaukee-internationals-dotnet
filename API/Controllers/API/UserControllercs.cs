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
    public class UserController : BasicCrudController<User>
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
        /// Returns instance of logic
        /// </summary>
        /// <returns></returns>
        public override IBasicCrudLogic<User> BasicCrudLogic() => _userLogic;
    }
}