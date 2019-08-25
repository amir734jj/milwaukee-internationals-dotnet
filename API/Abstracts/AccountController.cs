using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enums;
using Models.ViewModels.Identities;

namespace API.Abstracts
{
    public abstract class AbstractAccountController : Controller
    {
        public abstract UserManager<User> ResolveUserManager();

        public abstract SignInManager<User> ResolveSignInManager();

        public abstract RoleManager<IdentityRole<int>> ResolveRoleManager();

        public async Task<bool> Register(RegisterViewModel registerViewModel)
        {
            var user = new User
            {
                Fullname = registerViewModel.Fullname,
                UserName = registerViewModel.Username,
                Email = registerViewModel.Email,
                PhoneNumber = registerViewModel.PhoneNumber
            };

            var rslt1 = await ResolveUserManager().CreateAsync(user, registerViewModel.Password);

            var role = ResolveUserManager().Users.Any() ? UserRoleEnum.Basic.ToString() : UserRoleEnum.Admin.ToString();

            if (!await ResolveRoleManager().RoleExistsAsync(role))
            {
                await ResolveRoleManager().CreateAsync(new IdentityRole<int>(role));
            }
            
            var rslt2 = await ResolveUserManager().AddToRoleAsync(user, role);

            return rslt1.Succeeded && rslt2.Succeeded;
        }

        public async Task<bool> Login(LoginViewModel loginViewModel)
        {
            // Ensure the username and password is valid.
            var rslt = await ResolveUserManager().FindByNameAsync(loginViewModel.Username);

            if (rslt == null || !await ResolveUserManager().CheckPasswordAsync(rslt, loginViewModel.Password))
            {
                return false;
            }

            await ResolveSignInManager().SignInAsync(rslt, true);

            // Generate and issue a JWT token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, rslt.Email)
            };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name, ClaimTypes.Role);

            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.Now.AddDays(1),
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(principal), authProperties);

            return true;
        }

        public async Task Logout()
        {
            await ResolveSignInManager().SignOutAsync();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}