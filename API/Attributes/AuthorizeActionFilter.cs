﻿using System.Reflection;
using System.Threading.Tasks;
using API.Extensions;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Constants;

namespace API.Attributes
{
    public class AuthorizeActionFilter : IAsyncActionFilter
    {
        private readonly IIdentityLogic _identityLogic;

        public AuthorizeActionFilter(IIdentityLogic identityLogic)
        {
            _identityLogic = identityLogic;
        }
        
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = (Controller) context.Controller;
            var method = ((ControllerActionDescriptor) context.ActionDescriptor).MethodInfo;
            
            var controllerLevelAuthorize = controller.GetType().GetCustomAttribute<AuthorizeMiddlewareAttribute>();
            var actionLevelAuthorize = method.GetCustomAttribute<AuthorizeMiddlewareAttribute>();
            
            if (controllerLevelAuthorize == null && actionLevelAuthorize == null) return next();
            
            // Try to get username/password from session
            var userInfo = context.HttpContext.Session.GetUserInfo();

            // Validate username/password
            if (_identityLogic.IsAuthenticated(userInfo.Username, userInfo.Password))
            {
                return next();
            }

            var websiteUrl = context.HttpContext.Request.Host;

            // Redirect to not-authenticated
            context.HttpContext.Response.Redirect($"{websiteUrl}/Identity/NotAuthenticated");

            return Task.CompletedTask;
        }
    }
}