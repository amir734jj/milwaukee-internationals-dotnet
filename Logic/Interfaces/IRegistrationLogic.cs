using Models;

namespace Logic.Interfaces
{
    public interface IRegistrationLogic
    {
        bool RegisterDriver(Driver driver);

        bool RegisterStudent(Student student);
    }
}