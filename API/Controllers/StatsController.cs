using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[AuthorizeMiddleware(UserRoleEnum.Admin)]
[Route("[controller]")]
public class StatsController : Controller
{
    private readonly IStatsLogic _statsLogic;

    public StatsController(IStatsLogic statsLogic)
    {
        _statsLogic = statsLogic;
    }

    [HttpGet]
    [Route("")]
    [SwaggerOperation("Stats")]
    public async Task<IActionResult> Index()
    {
        return View(await _statsLogic.GetStats());
    }
}