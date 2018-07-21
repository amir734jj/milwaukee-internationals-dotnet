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
        public override IMapper GetMapper() => _mapper;
        
        /// <summary>
        /// Returns database context
        /// </summary>
        /// <returns></returns>
        public override DbContext GetDbContext() => _dbContext;

        /// <summary>
        /// Returns hosts entity
        /// </summary>
        /// <returns></returns>
        public override DbSet<Host> GetDbSet() => _dbContext.Hosts;
    }
}