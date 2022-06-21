using System.Linq;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;


namespace DAL.Profiles
{
    public class EventProfile :   IEntityProfile<Event>
    {
        public  void Update(Event entity, Event dto)
        {
            entity.Address = dto.Address;
            entity.Description = dto.Description;
            entity.Name = dto.Name;
            entity.DateTime = dto.DateTime;
            ((IEntityProfile<Event>)this).ModifyList(entity.Students, dto.Students, x => x.Id);
        }

        public IQueryable<Event> Include(IQueryable<Event> queryable)
        {
            return queryable
                .Include(x => x.Students)
                .ThenInclude(x => x.Student)
                .OrderBy(x => x.Name);
        }
    }
}