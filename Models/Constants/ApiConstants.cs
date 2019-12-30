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

        public static readonly string InviteCode = $"Tour{DateTime.UtcNow.Year}";

        public const string SiteUrl = "http://www.milwaukeeinternationals.com";

        public static readonly string[] AdminEmail =
        {
            "amirhesamyan@gmail.com",
            "asherimtiaz@gmail.com"
        };

        public const string SiteEmail = "tourofmilwaukee@gmail.com";

        /// <summary>
        ///     Available themes
        /// </summary>
        public static readonly IReadOnlyDictionary<string, string> Themes = new Dictionary<string, string>
        {
            ["cerulean"] = "cerulean.bootstrap.min.css",
            ["darkly"] = "darkly.bootstrap.min.css",
            ["lumen"] = "lumen.bootstrap.min.css",
            ["sandstone"] = "sandstone.bootstrap.min.css",
            ["spacelab"] = "spacelab.bootstrap.min.css",
            ["yeti"] = "yeti.bootstrap.min.css",
            ["cosmo"] = "cosmo.bootstrap.min.css",
            ["flatly"] = "flatly.bootstrap.min.css",
            ["paper"] = "paper.bootstrap.min.css",
            ["simplex"] = "simplex.bootstrap.min.css",
            ["superhero"] = "superhero.bootstrap.min.css",
            ["cyborg"] = "cyborg.bootstrap.min.css",
            ["journal"] = "journal.bootstrap.min.css",
            ["readable"] = "readable.bootstrap.min.css",
            ["slate"] = "slate.bootstrap.min.css",
            ["united"] = "united.bootstrap.min.css",
            ["default"] = ""
        };
    }
}