using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;

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
    public async Task<IActionResult> Index()
    {
        return View(await _statsLogic.GetStats());
    }

    [HttpGet]
    [Route("CountryDistribution")]
    public async Task<IActionResult> GetCountryDistribution()
    {
        return Ok(await _statsLogic.GetCountryDistribution());
    }
}