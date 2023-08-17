using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[AllowAnonymous]
[Route("[controller]")]
public class PlacesController : Controller
{
    [HttpGet]
    [Route("{year:int?}")]
    public IActionResult Place(int year = 2023)
    {
        return View();
    }
}