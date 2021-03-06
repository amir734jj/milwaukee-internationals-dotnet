using System.Linq;
using EfCoreRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL.Profiles
{
    public class StudentProfile : IEntityProfile<Student>
    {
        public void Update(Student entity, Student dto)
        {
            entity.DisplayId = dto.DisplayId;
            entity.Fullname = dto.Fullname;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.Major = dto.Major;
            entity.Country = dto.Country;
            entity.University = dto.University;
            entity.IsFamily = dto.IsFamily;
            entity.Interests = dto.Interests;
            entity.NeedCarSeat = dto.NeedCarSeat;
            entity.KosherFood = dto.KosherFood;
            entity.FamilySize = dto.FamilySize;
        }

        public IQueryable<Student> Include<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<Student>
        {
            return queryable
                .Include(x => x.Events)
                .Include(x => x.Driver)
                .Include(x => x.Driver.Host)
                .OrderBy(x => x.Fullname);
        }
    }
}