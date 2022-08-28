using System.Threading.Tasks;
using API.Attributes;
using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models.Enums;
using Swashbuckle.AspNetCore.Annotations;

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
    [SwaggerOperation("ApiEvents")]
    public async Task<IActionResult> Index()
    {
        return View(await _apiEventService.GetEvents());
    }
}