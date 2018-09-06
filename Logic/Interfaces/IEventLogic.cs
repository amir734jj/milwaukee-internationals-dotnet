using System.Threading.Tasks;
using Models;

namespace Logic.Interfaces
{
    public interface IEventLogic : IBasicCrudLogic<Event>
    {
        Task<bool> MapStudent(int eventId, int studentId);
        
        Task<bool> UnMapStudent(int eventId, int studentId);
    }
}