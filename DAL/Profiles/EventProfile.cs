using System.Linq;
using DAL.Interfaces;
using EfCoreRepository;
using Microsoft.EntityFrameworkCore;
using Models.Entities;


namespace DAL.Profiles
{
    public class EventProfile : EntityProfile<Event>, IEntityProfile<Event> 
    {
        public  override void Update(Event entity, Event dto)
        {
            entity.Address = dto.Address;
            entity.Description = dto.Description;
            entity.Name = dto.Name;
            entity.DateTime = dto.DateTime;
            ((IEntityProfile<Event>)this).ModifyList(entity.Students, dto.Students, x => x.Id);
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