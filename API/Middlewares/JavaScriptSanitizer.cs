using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Flurl;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Middlewares
{
    public class JavaScriptSanitizer : IAsyncActionFilter
    {
        /// <summary>
        ///     Regex to match XML tags
        /// </summary>
        private static readonly Regex Regex = new Regex(@"<([^\s]+)(\s[^>]*?)?(?<!\/)>");
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            using (var stream = new StreamReader(context.HttpContext.Request.Body))
            {
                var requestBodyStrEncoded = await stream.ReadToEndAsync();
                var requestBodyStrDecoded = Url.Decode(requestBodyStrEncoded, false);

                if (!Regex.Match(requestBodyStrDecoded).Success && !Regex.Match(requestBodyStrEncoded).Success)
                {
                    await next();
                }
                else
                {
                    var url = Url.Combine(context.HttpContext.Request.Host.Value, "/Error");

                    context.HttpContext.Response.Redirect(url);
                }
            }
        }
    }
}