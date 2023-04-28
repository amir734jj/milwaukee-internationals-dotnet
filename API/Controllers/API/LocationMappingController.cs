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
public class LocationMappingController : BasicCrudController<LocationMapping>
{
    private readonly ILocationMappingLogic _locationMappingLogic;

    /// <summary>
    /// Constructor dependency injection
    /// </summary>
    /// <param name="locationMappingLogic"></param>
    public LocationMappingController(ILocationMappingLogic locationMappingLogic)
    {
        _locationMappingLogic = locationMappingLogic;
    }

    /// <summary>
    /// Returns instance of logic
    /// </summary>
    /// <returns></returns>
    protected override IBasicCrudLogic<LocationMapping> BasicCrudLogic()
    {
        return _locationMappingLogic;
    }
    
    [AllowAnonymous]
    [HttpGet]
    [Route("")]
    [SwaggerOperation("GetAll")]
    [ProducesResponseType(typeof(IEnumerable<LocationMapping>), 200)]
    public override async Task<IActionResult> GetAll()
    {
        return Ok(await BasicCrudLogic().GetAll());
    }
}