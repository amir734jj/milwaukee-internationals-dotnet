using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Abstracts;
using DAL.Extensions;
using DAL.Interfaces;
using DAL.Utilities;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL
{
    public class EventDal : BasicCrudDalAbstract<Event>, IEventDal
    {
        private readonly EntityDbContext _dbContext;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="dbContext"></param>
        public EventDal(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Returns database context
        /// </summary>
        /// <returns></returns>
        protected override DbContext GetDbContext()
        {
            return _dbContext;
        }

        /// <summary>
        /// Returns hosts entity
        /// </summary>
        /// <returns></returns>
        protected override DbSet<Event> GetDbSet()
        {
            return _dbContext.Events;
        }

        public override async Task<Event> Update(int id, Event dto)
        {
            var entity = await Get(id);

            entity.Address = dto.Address;
            entity.Description = dto.Description;
            entity.Name = dto.Name;
            entity.DateTime = dto.DateTime;
            entity.Students = entity.Students.IdAwareUpdate(dto.Students, x => x.Id);

            return await base.Update(id, entity);
        }

        protected override IQueryable<Event> Intercept<TQueryable>(TQueryable queryable)
        {
            return queryable
                .Include(x => x.Students)
                .ThenInclude(x => x.Student)
                .OrderBy(x => x.Name);
        }
    }
}