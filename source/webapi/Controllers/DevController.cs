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

    [HttpPost]
    [Route("/api/regions")]
    public IActionResult Post([FromBody] CreateRegionRequest request)
    {
        try
        {
            var ctx = new AppDbContext(Configuration);
            RegionEntity region = new RegionEntity() { RegionId = request.RegionId };
            region.Latitude = request.LocationCoordination.Latitude;
            region.Longitude = request.LocationCoordination.Longitude;
            region.DisasterTypes = request.DisasterTypes;
            ctx.Regions.Add(region);
            ctx.SaveChanges();
            return Created("/api/regions", region);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

}
