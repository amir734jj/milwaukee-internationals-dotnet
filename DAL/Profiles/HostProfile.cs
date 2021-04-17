using System.Linq;
using EfCoreRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL.Profiles
{
    public class HostProfile : IEntityProfile<Host>
    {
        public void Update(Host entity, Host dto)
        {
            entity.Fullname = dto.Fullname;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.Address = dto.Address;
        }

        public IQueryable<Host> Include<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<Host>
        {
            return queryable
                .Include(x => x.Drivers)
                .ThenInclude(x => x.Students)
                .OrderBy(x => x.Fullname);
        }
    }
}