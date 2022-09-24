using System.Linq;
using EfCoreRepository;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL.Profiles
{
    public class DriverProfile : EntityProfile<Driver>
    {
        public DriverProfile()
        {
            MapAll();
        }

        protected override IQueryable<Driver> Include<TQueryable>(TQueryable queryable)
        {
            return queryable
                .Include(x => x.Host)
                .Include(x => x.Students)
                .OrderBy(x => x.Fullname);
        }
    }
}