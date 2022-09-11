using System.Linq;
using EfCoreRepository;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL.Profiles
{
    public class StudentProfile : EntityProfile<Student>
    {
        public StudentProfile()
        {
            MapAll(x => x.Driver);
        }

        public override IQueryable<Student> Include<TQueryable>(TQueryable queryable)
        {
            return queryable
                .Include(x => x.Events)
                .Include(x => x.Driver)
                .ThenInclude(x => x.Host)
                .OrderBy(x => x.Fullname);
        }
    }
}