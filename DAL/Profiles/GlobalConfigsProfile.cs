using EfCoreRepository;
using Models.Entities;

namespace DAL.Profiles;

public class GlobalConfigsProfile : EntityProfile<GlobalConfigs>
{
    public GlobalConfigsProfile()
    {
        MapAll();
    }
}