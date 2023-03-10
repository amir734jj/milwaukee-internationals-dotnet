using System.Threading.Tasks;
using Models.Entities;

namespace Logic.Interfaces;

public interface ILocationLogic: IBasicCrudLogic<Location>
{
    Task MoveRankUp(int id);
    
    Task MoveRankDown(int id);
}