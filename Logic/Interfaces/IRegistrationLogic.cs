﻿using Models;
using Models.Entities;

namespace Logic.Interfaces
{
    public interface IRegistrationLogic
    {
        bool RegisterDriver(Driver driver);

        bool RegisterStudent(Student student);
        
        bool RegisterHost(Host host);
    }
}