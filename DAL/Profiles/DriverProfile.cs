using System.Linq;
using EfCoreRepository;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL.Profiles
{
    public class DriverProfile : EntityProfile<Driver>
    {
        public override void Update(Driver entity, Driver dto)
        {
            entity.Fullname = dto.Fullname;
            entity.Role = dto.Role;
            entity.Phone = dto.Phone;
            entity.Email = dto.Email;
            entity.Capacity = dto.Capacity;
            entity.Navigator = dto.Navigator;
            entity.DisplayId = dto.DisplayId;
            entity.RequireNavigator = dto.RequireNavigator;
            entity.HaveChildSeat = dto.HaveChildSeat;
        }

        public override IQueryable<Driver> Include<TQueryable>(TQueryable queryable)
        {
            return queryable
                .Include(x => x.Host)
                .Include(x => x.Students)
                .OrderBy(x => x.Fullname);
        }
    }
}