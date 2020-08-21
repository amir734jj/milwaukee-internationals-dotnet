using System.Linq;
using EfCoreRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL.Profiles
{
    public class DriverProfile : IEntityProfile<Driver, int>
    {
        public Driver Update(Driver entity, Driver dto)
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

            return entity;
        }

        public IQueryable<Driver> Include<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<Driver>
        {
            return queryable
                .Include(x => x.Host)
                .ThenInclude(x => x.Drivers)
                .Include(x => x.Students)
                .OrderBy(x => x.Fullname);
        }
    }
}