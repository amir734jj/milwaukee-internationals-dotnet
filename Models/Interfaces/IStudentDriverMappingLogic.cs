using Models.ViewModels;

namespace Models.Interfaces
{
    public interface IStudentDriverMappingLogic
    {
        bool MapStudentToDriver(NewStudentDriverMappingViewModel newStudentDriverMappingViewModel);
        
        bool UnMapStudentToDriver(NewStudentDriverMappingViewModel newStudentDriverMappingViewModel);

        StudentDriverMappingViewModel MappingStatus();
    }
}