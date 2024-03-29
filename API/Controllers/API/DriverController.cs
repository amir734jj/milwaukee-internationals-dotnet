﻿using System.Threading.Tasks;
using API.Abstracts;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.API;

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
        
    [HttpGet]
    [Route("login/{driverId}")]
    [SwaggerOperation("DriverLogin")]
    public async Task<IActionResult> DriverLogin([FromRoute] string driverId)
    {
        var driver = await _driverLogic.FindByDriverId(driverId);

        if (driver == null)
        {
            return BadRequest("Failed to find the driver");
        }

        return Ok(driver);
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