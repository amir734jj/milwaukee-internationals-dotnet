﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models;

namespace Logic
{
    public class EventLogic : BasicCrudLogicAbstract<Event>, IEventLogic
    {
        private readonly IEventDal _eventDal;
        
        private readonly IStudentLogic _studentLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="eventDal"></param>
        /// <param name="studentLogic"></param>
        public EventLogic(IEventDal eventDal, IStudentLogic studentLogic)
        {
            _eventDal = eventDal;
            _studentLogic = studentLogic;
        }

        /// <summary>
        /// Returns instance of student DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudDal<Event> GetBasicCrudDal() => _eventDal;

        /// <summary>
        /// Assignes student to event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public async Task<bool> MapStudent(int eventId, int studentId)
        {
            // Gets the student by Id
            var student = await _studentLogic.Get(studentId);

            // Update the event
            await _eventDal.Update(eventId, @event =>
            {
                // Make sure it is not null or empty
                @event.Students = @event.Students ?? new List<Student>();
                
                // Add student
                @event.Students.Add(student);
            });

            return true;
        }
        
        /// <summary>
        /// Removed student from an event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public async Task<bool> UnMapStudent(int eventId, int studentId)
        {
            // Gets the student by Id
            var student = await _studentLogic.Get(studentId);

            // Update the event
            await _eventDal.Update(eventId, @event =>
            {
                // Remove student
                @event.Students?.Remove(student);
            });

            return true;
        }
    }
}