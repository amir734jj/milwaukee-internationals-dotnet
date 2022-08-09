using System.Linq;
using EfCoreRepository;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL.Profiles
{
    public class HostProfile : EntityProfile<Host>
    {
        public override void Update(Host entity, Host dto)
        {
            entity.Fullname = dto.Fullname;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.Address = dto.Address;
        }

        public override IQueryable<Host> Include<TQueryable>(TQueryable queryable)
        {
            return queryable
                .Include(x => x.Drivers)
                .ThenInclude(x => x.Students)
                .OrderBy(x => x.Fullname);
        }
    }
}