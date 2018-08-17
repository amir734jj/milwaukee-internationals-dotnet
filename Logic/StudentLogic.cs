using System.Linq;
using DAL.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models;
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
        public override IBasicCrudDal<Student> GetBasicCrudDal() => _studentDal;
        
        /// <summary>
        /// Make sure display ID is not null or empty
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override Student Save(Student instance)
        {
            // TODO: make this faster
            instance.DisplayId = GenerateDisplayId(instance, _studentDal.GetAll().Select(x => x.Id).Max() + 1);
            
            return base.Save(instance);
        }
    }
}