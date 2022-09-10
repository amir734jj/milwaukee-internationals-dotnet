using System.Linq;
using EfCoreRepository;
using Microsoft.EntityFrameworkCore;
using Models.Entities;


namespace DAL.Profiles
{
    public class EventProfile : EntityProfile<Event>
    {
        public EventProfile()
        {
            MapAll(x => x.Id);
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