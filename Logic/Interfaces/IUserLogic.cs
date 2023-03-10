using System.Threading.Tasks;
using Models.Entities;

namespace Logic.Interfaces;

public interface IUserLogic : IBasicCrudLogic<User>
{
    Task Disable(int id);
    Task Enable(int id);
}