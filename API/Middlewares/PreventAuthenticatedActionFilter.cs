using System.Reflection;
using System.Threading.Tasks;
using API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Middlewares
{
    public class PreventAuthenticatedActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var disallowAuthenticatedAttribute = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo
                .GetCustomAttribute<DisallowAuthenticatedAttribute>();
            
            if (context.HttpContext.User.Identity != null && disallowAuthenticatedAttribute != null && context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Index", "Home", new { });
            }
            else
            {
                await next();
            }
        }
    }
}