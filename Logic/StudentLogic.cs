using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models;
using Models.Entities;
using static Logic.Utilities.DisplayIdUtility;

namespace Logic
{
    public class StudentLogic : BasicCrudLogicAbstract<Student>, IStudentLogic
    {
        private readonly IStudentDal _studentDal;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="studentDal"></param>
        public StudentLogic(IStudentDal studentDal)
        {
            _studentDal = studentDal;
        }

        /// <summary>
        /// Returns instance of student DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudDal<Student> GetBasicCrudDal() => _studentDal;
        
        /// <summary>
        /// Make sure display ID is not null or empty
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override async Task<Student> Save(Student instance)
        {
            // TODO: make this faster
            instance.DisplayId = "Null";
            
            // Save student
            var retVal = await base.Save(instance);
            
            // Set the display id
            instance.DisplayId = GenerateDisplayId(instance, instance.Id);
            
            // Set the year
            instance.Year = DateTime.Now.Year;

            // Update
            await Update(instance.Id, instance);

            return retVal;
        }
    }
}