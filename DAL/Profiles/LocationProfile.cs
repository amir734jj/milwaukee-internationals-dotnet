using EfCoreRepository;
using Models.Entities;

namespace DAL.Profiles;

public class LocationProfile : EntityProfile<Location>
{
    public LocationProfile()
    {
        MapAll(location => location.Year);
    }
}