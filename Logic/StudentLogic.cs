﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Interfaces;
using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Constants;
using Models.Entities;
using static Logic.Utilities.RegistrationUtility;

namespace Logic
{
    public class StudentLogic : BasicCrudLogicAbstract<Student>, IStudentLogic
    {
        private readonly IBasicCrud<Student> _dal;
        private readonly GlobalConfigs _globalConfigs;
        private readonly IApiEventService _apiEventService;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="globalConfigs"></param>
        /// <param name="apiEventService"></param>
        public StudentLogic(IEfRepository repository, GlobalConfigs globalConfigs, IApiEventService apiEventService)
        {
            _dal = repository.For<Student>();
            _globalConfigs = globalConfigs;
            _apiEventService = apiEventService;
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
            
            // Normalize phone number
            student.Phone = NormalizePhoneNumber(student.Phone);

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
            
            // Update only subset of properties
            return await base.Update(id, x =>
            {
                x.Fullname = student.Fullname;
                x.DisplayId = student.DisplayId;
                x.Email = student.Email;
                x.Phone = student.Phone;
                x.University = student.University;
                x.Major = student.Major;
                x.Country = student.Country;
                x.Interests = student.Interests;
                x.IsFamily = student.IsFamily;
                x.KosherFood = student.KosherFood;
                x.MaskPreferred = student.MaskPreferred;
            });
        }

        protected override IBasicCrud<Student> Repository()
        {
            return _dal;
        }
        
        protected override IApiEventService ApiEventService()
        {
            return _apiEventService;
        }

        public override async Task<IEnumerable<Student>> GetAll(string sortBy = null, bool? descending = null)
        {
            return (await base.GetAll(sortBy, descending)).Where(x => x.Year == _globalConfigs.YearValue);
        }
    }
}