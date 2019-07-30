using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Abstracts;
using DAL.Interfaces;
using DAL.Utilities;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL
{
    public class EventDal : BasicCrudDalAbstract<Event>, IEventDal
    {
        private readonly EntityDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public EventDal(EntityDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns IMapper
        /// </summary>
        /// <returns></returns>
        protected override IMapper GetMapper()
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