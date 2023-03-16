using System.Linq;
using EfCoreRepository;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL.Profiles;

public class LocationMappingProfile: EntityProfile<LocationMapping>
{
    public LocationMappingProfile()
    {
        Map(x => x.SourceId);
        Map(x => x.SinkId);
        Map(x => x.Year);
        Map(x => x.Description);
    }

    protected override IQueryable<LocationMapping> Include<TQueryable>(TQueryable queryable)
    {
        return queryable
            .Include(x => x.Source)
            .Include(x => x.Sink);
    }
}