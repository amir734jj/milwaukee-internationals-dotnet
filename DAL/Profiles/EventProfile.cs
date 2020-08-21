using System.Linq;
using EfCoreRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL.Profiles
{
    public class EventProfile : IEntityProfile<Event, int>
    {
        private readonly IEntityProfileAuxiliary _auxiliary;

        public EventProfile(IEntityProfileAuxiliary auxiliary)
        {
            _auxiliary = auxiliary;
        }
        
        public Event Update(Event entity, Event dto)
        {
            entity.Address = dto.Address;
            entity.Description = dto.Description;
            entity.Name = dto.Name;
            entity.DateTime = dto.DateTime;
            entity.Students = _auxiliary.ModifyList<EventStudentRelationship, int>(entity.Students, dto.Students);

            return entity;
        }

        public IQueryable<Event> Include<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<Event>
        {
            return queryable
                .Include(x => x.Students)
                .ThenInclude(x => x.Student)
                .OrderBy(x => x.Name);
        }
    }
}