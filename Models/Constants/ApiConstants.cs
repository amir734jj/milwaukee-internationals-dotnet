using System;

namespace Models.Constants
{
    public static class ApiConstants
    {
        public const string AuthenticationSessionCookieName = "AuthenticationCookie";

        public static readonly string InviteCode = $"Tour{DateTime.UtcNow.Year}";

        public const string SiteUrl = "http://www.milwaukeeinternationals.com";

        public static readonly string[] AdminEmail =
        {
            "amirhesamyan@gmail.com",
            "asherimtiaz@gmail.com"
        };

        public const string SiteEmail = "tourofmilwaukee@gmail.com";
    }
}