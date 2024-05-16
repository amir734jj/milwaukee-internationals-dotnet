using System.Collections.Generic;
using System.Threading.Tasks;
using Models.ViewModels.EventService;

namespace DAL.Interfaces;

public interface IApiEventService
{
    Task RecordEvent(string description);

    IEnumerable<ApiEvent> GetEvents();
}