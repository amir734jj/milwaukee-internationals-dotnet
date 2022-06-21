using System.Linq;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL.Profiles
{
    public class DriverProfile : IEntityProfile<Driver>
    {
        public void Update(Driver entity, Driver dto)
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

        public IQueryable<Driver> Include(IQueryable<Driver> queryable)
        {
            return queryable
                .Include(x => x.Host)
                .ThenInclude(x => x.Drivers)
                .Include(x => x.Students)
                .OrderBy(x => x.Fullname);
        }
    }
}