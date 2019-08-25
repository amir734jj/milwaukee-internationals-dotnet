using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Models.Entities;
using Models.Enums;

namespace API.Extensions
{
    /// <summary>
    /// UserInfo Struct
    /// </summary>
    public struct UserInfo
    {
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public UserRoleEnum UserRoleEnum { get; set; }
    }
    
    public class HttpRequestUtility {
    
        private readonly UserManager<User> _userManager;
        
        private readonly SignInManager<User> _signInManager;

        private readonly HttpContext _ctx;
        
        public HttpRequestUtility(UserManager<User> userManager, SignInManager<User> signInManager, HttpContext ctx)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _ctx = ctx;
        }

        /// <summary>
        /// Extension method to quickly get the username/password
        /// </summary>
        /// <returns></returns>
        public async Task<UserInfo> GetUserInfo()
        {
            var user = await _userManager.GetUserAsync(_ctx.User);

            return new UserInfo
            {
                Username = user?.UserName,
                Password = user?.PasswordHash,
                UserRoleEnum = UserRoleEnum.Admin
            };
        }

        /// <summary>
        /// Extension method to check whether user is logged in or not
        /// </summary>
        /// <returns></returns>
        public bool IsAuthenticated()
        {
            return _signInManager.IsSignedIn(_ctx.User);
        }
    }
    
    public class HttpRequestUtilityBuilder
    {
        private readonly UserManager<User> _userManager;
        
        private readonly SignInManager<User> _signInManager;

        public HttpRequestUtilityBuilder(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public HttpRequestUtility For(HttpContext ctx)
        {
            return new HttpRequestUtility(_userManager, _signInManager, ctx);
        }
    }
}