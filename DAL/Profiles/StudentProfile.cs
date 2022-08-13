using System.Linq;
using EfCoreRepository;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL.Profiles
{
    public class StudentProfile : EntityProfile<Student>
    {
        public override void Update(Student entity, Student dto)
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
            entity.IsPresent = dto.IsPresent;
            entity.FamilySize = dto.FamilySize;
            entity.MaskPreferred = dto.MaskPreferred;
            entity.Year = dto.Year;
            entity.DriverRefId = dto.DriverRefId;
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