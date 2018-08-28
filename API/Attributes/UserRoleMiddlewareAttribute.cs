
using System;
using Models.Enums;

namespace API.Attributes
{
    public class UserRoleMiddlewareAttribute : Attribute
    {
        public UserRoleEnum UserRoleEnum { get; }
        
        public UserRoleMiddlewareAttribute(UserRoleEnum userRoleEnum)
        {
            UserRoleEnum = userRoleEnum;
        }
    }
}