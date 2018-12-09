using System;
using Microsoft.AspNetCore.Http;
using Models.Constants;
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
    
    public static class SessionExtension
    {
        /// <summary>
        /// Extension method to quickly get the username/password
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static UserInfo GetUserInfo(this ISession session)
        {
            var role = session.GetString(ApiConstants.UserRole);
            
            return new UserInfo
            {
                Username = session.GetString(ApiConstants.Username),
                Password = session.GetString(ApiConstants.Password),
                UserRoleEnum = !string.IsNullOrWhiteSpace(role) ? Enum.Parse<UserRoleEnum>(role) : default(UserRoleEnum)
            };
        }

        /// <summary>
        /// Extension method to check whether user is logged in or not
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool IsAuthenticated(this ISession session) => session.GetString(ApiConstants.Authenticated.Key) == ApiConstants.Authenticated.Value;
    }
}