using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
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
    public IActionResult Regions([FromBody] CreateRegionRequest[] request)
    {
        try
        {
            //Request Log
            Log.Information("Request {request}", JsonConvert.SerializeObject(request));
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
                // string[] disaster = ["Flood", "Earthquake", "Wildfire"];
                // string[] typesChecker = req.DisasterTypes.Where(t => !disaster.Contains(t)).ToArray();
                // if (typesChecker.Length > 0 || req.DisasterTypes.Length == 0)
                // {
                //     BaseResponse res = new BaseResponse(false, "System Only Support Flood,Earthquake and Wildfire.");
                //     Log.Error("Error Message: {ErrorMessage}", res.ErrorMessage);
                //     return BadRequest(res);
                // }

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

    [HttpPost]
    [Route("/api/alert-settings")]
    public IActionResult AlertSettings([FromBody] CreateAlertSettingRequest[] request)
    {
        try
        {
            //Request Log
            Log.Information("Request {request}", JsonConvert.SerializeObject(request));
            // > Setup Context
            var ctx = new AppDbContext(Configuration);
            // ====== Loop ======
            foreach (var req in request)
            {
                // Region is not Exist ==> 400
                RegionEntity? region = ctx.Regions.FirstOrDefault(r => r.RegionId.Equals(req.RegionId) & r.DisasterTypes.Contains(req.DisasterType));
                if (region == null)
                {
                    BaseResponse res = new BaseResponse(false, "Region ID or Disaster Type is not Exist");
                    Log.Error("Error Message: {ErrorMessage}", res.ErrorMessage);
                    return BadRequest(res);
                }

                AlertSettingEntity alert = new AlertSettingEntity() { RegionID = region };
                alert.DisasterType = req.DisasterType;
                alert.ThresholdScore = req.ThresholdScore;
                ctx.AlertSettings.Add(alert);

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

    [HttpGet]
    [Route("/api/disaster-risks")]
    public async Task<IActionResult> FectchDisaster()
    {
        try
        {
            // Setup Ctx
            AppDbContext ctx = new AppDbContext(Configuration);
            // Cache here
            // get all Alert
            AlertSettingEntity[] alerts = ctx.AlertSettings.Include(a => a.RegionID).ToArray();
            //loop
            foreach (AlertSettingEntity alert in alerts)
            {
                AlertEntity alertEntity = new AlertEntity()
                {
                    RegionId = alert.RegionID,
                    DisasterType = alert.DisasterType,
                    RiskScore = await RiskCalculationService.CalcurateAsync(Configuration, alert.DisasterType, alert.RegionID.Latitude, alert.RegionID.Longitude),
                    AlertTriggered = false,
                };
                alertEntity.RiskLevel = RiskCalculationService.GetLevel(alert.ThresholdScore, alertEntity.RiskScore);
                ctx.Alerts.Add(alertEntity);
            }
            ctx.SaveChanges();

            return Ok(ctx.Alerts.Select(a => new DisasterRiskResponse(a)).ToArray());
        }
        catch (Exception ex)
        {
            Log.Error("Error Message: {ErrorMessage}", ex.Message);
            return Problem(ex.Message);
        }
    }
}
