using Microsoft.AspNetCore.Authorization;
using Models.Enums;

namespace API.Attributes
{
    public class UserRoleMiddlewareAttribute : AuthorizeAttribute
    {
        private UserRoleEnum UserRoleEnum { get; }
        
        public UserRoleMiddlewareAttribute(UserRoleEnum userRoleEnum)
        {
            Roles = userRoleEnum.ToString();
            
            UserRoleEnum = userRoleEnum;
        }
    }
}