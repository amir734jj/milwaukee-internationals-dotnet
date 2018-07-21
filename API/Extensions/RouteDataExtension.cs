using Microsoft.AspNetCore.Routing;

namespace API.Extensions
{
    public static class RouteDataExtension
    {
        /// <summary>
        /// Returns the controller from route data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetAction(this RouteData data) => data.Values["Controller"].ToString();
    }
}