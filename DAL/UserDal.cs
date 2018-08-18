using AutoMapper;
using DAL.Abstracts;
using DAL.Interfaces;
using DAL.Utilities;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DAL
{
    public class UserDal :  BasicCrudDalAbstract<User>, IUserDal
    {
        private readonly EntityDbContext _dbContext;
        
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public UserDal(EntityDbContext dbContext, IMapper mapper)
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
        /// Returns students entity
        /// </summary>
        /// <returns></returns>
        protected override DbSet<User> GetDbSet() => _dbContext.Users;
    }
}