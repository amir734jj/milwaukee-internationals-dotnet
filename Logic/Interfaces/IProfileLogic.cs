using System.Threading.Tasks;
using Models.Entities;
using Models.ViewModels;

namespace Logic.Interfaces;

public interface IProfileLogic
{
    ProfileViewModel ResolveProfile(User user);

    Task UpdateUser(ProfileViewModel profile);
}