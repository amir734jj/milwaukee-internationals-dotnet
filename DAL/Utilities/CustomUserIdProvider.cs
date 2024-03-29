using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Models.Entities;

namespace DAL.Utilities;

public class CustomUserIdProvider : IUserIdProvider
{
    private readonly UserManager<User> _userManager;

    public CustomUserIdProvider(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public string GetUserId(HubConnectionContext connection)
    {
        return _userManager.FindByNameAsync(connection.User.Identity!.Name).Result.Id.ToString();
    }
}