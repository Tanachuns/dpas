using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Serilog;
using server.Models;
using webapi.Models;
using webapi.Models.Entities;

namespace webapi.Controllers;

public class DPAController : Controller
{
    private readonly IConfiguration Configuration;

    public DPAController(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }
    [HttpPost]
    [Route("/api/regions")]
    public IActionResult Post([FromBody] CreateRegionRequest[] request)
    {
        try
        {
            //Request Log
            Log.Information("Request", request);
            // > Setup Context
            var ctx = new AppDbContext(Configuration);
            // ====== Loop ======
            foreach (var req in request)
            {
                // > ID Already Exist => 400
                RegionEntity[] _region = ctx.Regions.Where(r => r.RegionId.Equals(req.RegionId)).ToArray();
                if (_region.Length > 0)
                {
                    BaseResponse res = new BaseResponse(false, "Region ID Already Exist");
                    Log.Error("Error Message: {ErrorMessage}", res.ErrorMessage);
                    return BadRequest(res);
                }
                // > Diaster Type Not Support  ==> 400
                string[] disaster = ["Flood", "Earthquake", "Wildfire"];
                string[] typesChecker = req.DisasterTypes.Where(t => !disaster.Contains(t)).ToArray();
                if (typesChecker.Length > 0 || req.DisasterTypes.Length == 0)
                {
                    BaseResponse res = new BaseResponse(false, "System Only Support Flood,Earthquake and Wildfire.");
                    Log.Error("Error Message: {ErrorMessage}", res.ErrorMessage);
                    return BadRequest(res);
                }

                // > Map Req and Ent
                RegionEntity region = new RegionEntity() { RegionId = req.RegionId };
                region.Latitude = req.LocationCoordination.Latitude;
                region.Longitude = req.LocationCoordination.Longitude;
                region.DisasterTypes = req.DisasterTypes;
                ctx.Regions.Add(region);
            }
            // > Save ==> 203
            int changes = ctx.SaveChanges();
            Log.Information("Create Success, {changes} Rows Affected", changes);
            return Created("/api/regions", new BaseResponse(true));
        }
        catch (Exception ex)
        {
            Log.Error("Error Message: {ErrorMessage}", ex.Message);
            return Problem(ex.Message);
        }
    }

}
