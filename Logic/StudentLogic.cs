using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Constants;
using Models.Entities;
using static Logic.Utilities.DisplayIdUtility;

namespace Logic
{
    public class StudentLogic : BasicCrudLogicAbstract<Student>, IStudentLogic
    {
        private readonly IBasicCrud<Student> _dal;
        private readonly GlobalConfigs _globalConfigs;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="globalConfigs"></param>
        public StudentLogic(IEfRepository repository, GlobalConfigs globalConfigs)
        {
            _dal = repository.For<Student>();
            _globalConfigs = globalConfigs;
        }
        

        /// <inheritdoc />
        /// <summary>
        /// Make sure display ID is not null or empty
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public override async Task<Student> Save(Student student)
        {
            var allStudents = (await GetAll(DateTime.UtcNow.Year)).ToList();

            if (_globalConfigs.DisallowDuplicateStudents && allStudents.Any(x =>
                    x.Fullname.Equals(student.Fullname, StringComparison.OrdinalIgnoreCase) &&
                    x.Email.Equals(student.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("Student already registered");
            }
            
            student.DisplayId = "Null";

            // Set the year
            student.Year = DateTime.UtcNow.Year;
            student.RegisteredOn = DateTimeOffset.Now;

            var count = allStudents.Count;

            student.DisplayId = GenerateDisplayId(student, count);

            // If student is not a family then family size should be zero
            if (!student.IsFamily)
            {
                student.FamilySize = 0;
            }
            
            // Save student
            var retVal = await base.Save(student);

            return retVal;
        }

        /// <inheritdoc />
        /// <summary>
        /// Edit student
        /// </summary>
        /// <param name="id"></param>
        /// <param name="student"></param>
        /// <returns></returns>
        public override async Task<Student> Update(int id, Student student)
        {
            // If student is not a family then family size should be zero
            if (!student.IsFamily)
            {
                student.FamilySize = 0;
            }

            return await base.Update(id, student);
        }

        protected override IBasicCrud<Student> Repository()
        {
            return _dal;
        }

        public override async Task<IEnumerable<Student>> GetAll(string sortBy = null, bool? descending = null)
        {
            return (await base.GetAll(sortBy, descending)).Where(x => x.Year == _globalConfigs.YearValue);
        }
    }
}