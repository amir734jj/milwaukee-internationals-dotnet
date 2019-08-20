using Microsoft.AspNetCore.Hosting;

namespace API.Extensions
{
    public static class HostingEnvironmentExtension
    {
        /// <summary>
        /// Test whether environment is localhost
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static bool IsLocalhost(this IHostingEnvironment environment)
        {
            return environment.IsEnvironment("Localhost");
        }
    }
}