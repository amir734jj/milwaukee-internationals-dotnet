using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Models.Enums;

namespace API.Attributes
{
    public class UserRoleMiddlewareAttribute : AuthorizeAttribute
    {
        public UserRoleMiddlewareAttribute(params UserRoleEnum[] userRoleEnums)
        {
            Roles = string.Join(',', userRoleEnums.Select(x => x.ToString()));
        }
    }
}