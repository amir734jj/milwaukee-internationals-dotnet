using Models.ViewModels;

namespace Logic.Interfaces
{
    public interface IStudentDriverMappingLogic
    {
        bool MapStudentToDriver(NewStudentDriverMappingViewModel newStudentDriverMappingViewModel);
        
        bool UnMapStudentToDriver(NewStudentDriverMappingViewModel newStudentDriverMappingViewModel);

        StudentDriverMappingViewModel MappingStatus();
    }
}