﻿using API.Abstracts;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace API.Controllers.API
{
    [AuthorizeMiddleware]
    [Route("api/[controller]")]
    public class DriverController : BasicCrudController<Driver>
    {
        private readonly IDriverLogic _driverLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="driverLogic"></param>
        public DriverController(IDriverLogic driverLogic)
        {
            _driverLogic = driverLogic;
        }

        /// <summary>
        /// Returns instance of logic
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudLogic<Driver> BasicCrudLogic()
        {
            return _driverLogic;
        }
    }
}