using API.Attributes;
using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;

namespace API.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[AuthorizeMiddleware(UserRoleEnum.Admin)]
[Route("[controller]")]
public class ApiEventsController : Controller
{
    private readonly IApiEventService _apiEventService;

    public ApiEventsController(IApiEventService apiEventService)
    {
        _apiEventService = apiEventService;
    }

    [HttpGet]
    [Route("")]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet]
    [Route("latest")]
    public IActionResult GetLatestApiEvents()
    {
        return Ok(_apiEventService.GetEvents());
    }
}