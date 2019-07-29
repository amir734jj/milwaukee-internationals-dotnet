using System;
using System.Collections.Generic;

namespace Models.Constants
{
    public static class ApiConstants
    {
        public const string AuthenticationSessionCookieName = "AuthenticationCookie";
        
        /// <summary>
        /// Authenticated token
        /// </summary>
        public static readonly KeyValuePair<string, string> Authenticated
            = new KeyValuePair<string, string>("Authenticated", Guid.NewGuid().ToString());
        
        public const string Username = "Username";

        public const string Password = "Password";
        
        public const string UserRole = "UserRole";

        public static readonly string InviteCode = $"Tour{DateTime.Now.Year}";

        public const string WebSiteUrl = "http://www.milwaukeeinternationals.com";
        
        public const string WebSiteApiUrl = "http://www.milwaukeeinternationals.com/api";

        public static readonly string[] AdminEmail = {
            "amirhesamyan@gmail.com",
            "asherimtiaz@gmail.com"
        };

        public const string WebSiteEmail = "tourofmilwaukee@gmail.com";
    }
}