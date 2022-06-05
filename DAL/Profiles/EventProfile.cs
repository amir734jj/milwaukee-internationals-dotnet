using System.Linq;
using EfCoreRepository;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL.Profiles
{
    public class EventProfile :  EntityProfile<Event>
    {
        public override void Update(Event entity, Event dto)
        {
            entity.Address = dto.Address;
            entity.Description = dto.Description;
            entity.Name = dto.Name;
            entity.DateTime = dto.DateTime;
            ModifyList(entity.Students, dto.Students, x => x.Id);
        }

        public override IQueryable<Event> Include<TQueryable>(TQueryable queryable)
        {
            return queryable
                .Include(x => x.Students)
                .ThenInclude(x => x.Student)
                .OrderBy(x => x.Name);
        }
    }
}