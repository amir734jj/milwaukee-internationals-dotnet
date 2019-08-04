using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Abstracts;
using DAL.Interfaces;
using DAL.Utilities;
using EntityUpdater.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL
{
    public class EventDal : BasicCrudDalAbstract<Event>, IEventDal
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAssignmentUtility _mapper;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public EventDal(EntityDbContext dbContext, IAssignmentUtility mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Returns IAssignmentUtility
        /// </summary>
        /// <returns></returns>
        protected override IAssignmentUtility Mapper()
        {
            return _mapper;
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

        public override async Task<IEnumerable<Event>> GetAll()
        {
            return await GetDbSet()
                .Include(x => x.Students)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}