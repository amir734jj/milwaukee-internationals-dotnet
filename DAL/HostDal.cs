using System.Linq;
using AutoMapper;
using DAL.Abstracts;
using DAL.Interfaces;
using DAL.Utilities;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DAL
{
    public class HostDal : BasicCrudDalAbstract<Host>, IHostDal
    {
        private readonly EntityDbContext _dbContext;
        
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public HostDal(EntityDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns IMapper
        /// </summary>
        /// <returns></returns>
        protected override IMapper GetMapper() => _mapper;
        
        /// <summary>
        /// Returns database context
        /// </summary>
        /// <returns></returns>
        protected override DbContext GetDbContext() => _dbContext;

        /// <summary>
        /// Returns hosts entity
        /// </summary>
        /// <returns></returns>
        protected override DbSet<Host> GetDbSet() => _dbContext.Hosts;
        
        /// <summary>
        /// Override to include related entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Host Get(int id) => GetDbSet().Include(x => x.Drivers).FirstOrDefault();
    }
}