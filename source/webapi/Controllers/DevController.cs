using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using server.Models;
using webapi.Models;
using webapi.Models.Entities;

namespace webapi.Controllers;

public class DevController : Controller
{
    private readonly IConfiguration Configuration;

    public DevController(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }
    [HttpGet]
    [Route("/api/regions")]
    public IActionResult Get()
    {
        Log.Information("test");
        var ctx = new AppDbContext(Configuration);
        RegionEntity[] region = ctx.Regions.ToArray();
        return Ok(region);
    }



}
