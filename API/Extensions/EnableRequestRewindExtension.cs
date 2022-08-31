using API.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace API.Extensions
{
    public static class EnableRequestRewindExtension
    {
        public static IApplicationBuilder UseEnableRequestRewind(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<EnableRequestRewindMiddleware>();

            return builder;
        }
    }
}