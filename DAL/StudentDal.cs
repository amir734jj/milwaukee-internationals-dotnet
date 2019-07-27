using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Abstracts;
using DAL.Extensions;
using DAL.Interfaces;
using DAL.Utilities;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Constants;
using Models.Entities;

namespace DAL
{
    public class StudentDal : BasicCrudDalAbstract<Student>, IStudentDal
    {
        private readonly EntityDbContext _dbContext;
        
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public StudentDal(EntityDbContext dbContext, IMapper mapper)
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
        protected override DbSet<Student> GetDbSet() => _dbContext.Students;

        /// <summary>
        /// Override to include related entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<Student> Get(int id) => await GetDbSet()
            .Include(x => x.Driver)
            .Include(x => x.Driver.Host)
            .FirstOrDefaultAsync(x => x.Id == id);

        /// <summary>
        /// Override to include related entity
        /// </summary>
        /// <returns></returns>
        public override async Task<IEnumerable<Student>> GetAll() => await GetDbSet()
            .Include(x => x.Driver)
            .Include(x => x.Driver.Host)
            .OrderBy(x => x.Fullname)
            .ToListAsync();
    }
}