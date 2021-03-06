﻿using System;
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
        private readonly IBasicCrud<Student> _studentDal;
        
        private readonly GlobalConfigs _globalConfigs;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="globalConfigs"></param>
        public StudentLogic(IEfRepository repository, GlobalConfigs globalConfigs)
        {
            _studentDal = repository.For<Student>();
            _globalConfigs = globalConfigs;
        }

        /// <summary>
        /// Returns instance of student DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrud<Student> GetBasicCrudDal()
        {
            return _studentDal;
        }

        /// <inheritdoc />
        /// <summary>
        /// Make sure display ID is not null or empty
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public override async Task<Student> Save(Student student)
        {
            student.DisplayId = "Null";

            // Set the year
            student.Year = DateTime.UtcNow.Year;

            var count = (await base.GetAll(DateTime.UtcNow.Year)).Count();

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
        public override Task<Student> Update(int id, Student student)
        {
            // If student is not a family then family size should be zero
            if (!student.IsFamily)
            {
                student.FamilySize = 0;
            }

            return base.Update(id, student);
        }

        public override async Task<IEnumerable<Student>> GetAll()
        {
            return (await base.GetAll()).Where(x => x.Year == _globalConfigs.YearValue);
        }
    }
}