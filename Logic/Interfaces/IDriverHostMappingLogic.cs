using Models.ViewModels;

namespace Logic.Interfaces
{
    public interface IDriverHostMappingLogic
    {
        bool MapDriverToHost(NewDriverHostMappingViewModel newDriverHostMappingViewModel);
        
        bool UnMapDriverToHost(NewDriverHostMappingViewModel newDriverHostMappingViewModel);

        DriverHostMappingViewModel MappingStatus();
    }
}