using AutoMapper;
using Models.Constants;
using Models.ViewModels.Config;

namespace Models.Profiles;

public class GlobalConfigsMappingProfile: Profile
{
    public GlobalConfigsMappingProfile()
    {
        CreateMap<GlobalConfigs, GlobalConfigViewModel>();
        CreateMap<GlobalConfigViewModel, GlobalConfigs>();
    }
}