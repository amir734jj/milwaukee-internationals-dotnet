﻿using System.Threading.Tasks;
using API.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AuthorizeMiddleware]
    [Route("[controller]")]
    public class MappingController : Controller
    {
        // GET the view
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        /// <summary>
        /// Returns Student-Driver Mapping view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("StudentDriverMapping")]
        public async Task<IActionResult> StudentDriverMapping()
        {
            return View();
        }

        /// <summary>
        /// Returns Driver-Host Mapping view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("DriverHostMapping")]
        public async Task<IActionResult> DriverHostMapping()
        {
            return View();
        }
    }
}