using System.Threading.Tasks;
using Models.Entities;

namespace Logic.Interfaces;

public interface IRegistrationLogic
{
    Task RegisterDriver(Driver driver);

    Task RegisterStudent(Student student);
        
    Task<bool> IsRegisterStudentOpen();
        
    Task RegisterHost(Host host);

    Task RegisterEvent(Event @event);
    
    Task RegisterLocation(Location location);
}