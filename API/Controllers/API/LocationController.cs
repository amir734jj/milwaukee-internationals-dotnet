﻿using System.Collections.Generic;
using System.Threading.Tasks;
using API.Abstracts;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.API;

[AuthorizeMiddleware]
[Route("api/[controller]")]
public class LocationController : BasicCrudController<Location>
{
    private readonly ILocationLogic _locationLogic;

    /// <summary>
    /// Constructor dependency injection
    /// </summary>
    /// <param name="locationLogic"></param>
    public LocationController(ILocationLogic locationLogic)
    {
        _locationLogic = locationLogic;
    }

    /// <summary>
    /// Returns instance of logic
    /// </summary>
    /// <returns></returns>
    protected override IBasicCrudLogic<Location> BasicCrudLogic()
    {
        return _locationLogic;
    }
    
    [AllowAnonymous]
    [HttpGet]
    [Route("")]
    [SwaggerOperation("GetAll")]
    [ProducesResponseType(typeof(IEnumerable<Location>), 200)]
    public override async Task<IActionResult> GetAll()
    {
        return Ok(await BasicCrudLogic().GetAll());
    }
}