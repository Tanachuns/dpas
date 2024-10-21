using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using server.Models;
using StackExchange.Redis;
using webapi.Models;
using webapi.Models.Entities;
using static LineMessageService;
using static RiskCalculationService;

namespace webapi.Controllers;


public class DPAController : Controller
{
    private readonly IConfiguration Configuration;

    public DPAController(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }
    [HttpGet]
    [Route("")]
    public IActionResult GetAPI()
    {
        return Ok("Disaster Prediction and Alert System API");
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
                //check disasertype
                if (req.DisasterTypes == null || req.DisasterTypes.Length <= 0)
                {
                    BaseResponse res = new BaseResponse(false, "Disaster type is required.");
                    Log.Error("Error Message: {ErrorMessage}", res.ErrorMessage);
                    return BadRequest(res);
                }

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
            BaseResponse res = new BaseResponse(false, ex.Message);
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
                AlertSettingEntity alert;
                AlertSettingEntity alertSetting = ctx.AlertSettings.FirstOrDefault(a => a.DisasterType == req.DisasterType & a.RegionID.RegionId == req.RegionId);
                if (alertSetting == null)
                {
                    alert = new AlertSettingEntity()
                    {
                        RegionID = region,
                        DisasterType = req.DisasterType,
                        ThresholdScore = req.ThresholdScore
                    };
                    ctx.AlertSettings.Add(alert);
                }
                else
                {
                    alertSetting.ThresholdScore = req.ThresholdScore;
                    alert = alertSetting;
                    ctx.AlertSettings.Update(alert);
                }


            }
            // > Save ==> 203
            int changes = ctx.SaveChanges();
            Log.Information("Create Success, {changes} Rows Affected", changes);
            return Created("/api/regions", new BaseResponse(true));
        }
        catch (Exception ex)
        {
            Log.Error("Error Message: {ErrorMessage}", ex.Message);
            BaseResponse res = new BaseResponse(false, ex.Message);
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
            // get all Alert
            AlertSettingEntity[] alerts = ctx.AlertSettings.Include(a => a.RegionID).ToArray();
            //loop
            List<AlertEntity> alertEntities = new List<AlertEntity>();
            RiskCalculationConfig riskConfig = new RiskCalculationConfig()
            {
                WeatherBaseUrl = Configuration["Api:Weather:BaseUrl"] ?? throw new Exception("Configuation is null."),
                WeatherApiKey = Configuration["Api:Weather:ApiKey"] ?? throw new Exception("Configuation is null."),
                USGSBaseUrl = Configuration["Api:USGS:BaseUrl"] ?? throw new Exception("Configuation is null."),
                RedisConnectionString = Configuration["ConnectionStrings:RedisConnection"] ?? throw new Exception("Configuation is null.")
            };
            foreach (AlertSettingEntity alert in alerts)
            {
                AlertEntity alertEntity = new AlertEntity()
                {
                    RegionId = alert.RegionID,
                    DisasterType = alert.DisasterType,
                    RiskScore = await CalcurateAsync(riskConfig, alert.DisasterType, alert.RegionID.Latitude, alert.RegionID.Longitude),
                    AlertTriggered = false,
                };
                alertEntity.RiskLevel = GetLevel(alert.ThresholdScore, alertEntity.RiskScore);
                alertEntities.Add(alertEntity);
            }
            ctx.Alerts.AddRange(alertEntities);
            ctx.SaveChanges();

            return Ok(alertEntities.Select(a => new DisasterRiskResponse(a)).ToArray());
        }
        catch (Exception ex)
        {
            Log.Error("Error Message: {ErrorMessage}", ex.Message);
            BaseResponse res = new BaseResponse(false, ex.Message);
            return Problem(ex.Message);
        }
    }

    [HttpPost]
    [Route("/api/alerts/send")]
    public async Task<IActionResult> SendDisaster()
    {
        try
        {
            LineMessageService messageService = new LineMessageService();
            LineMessageConfig config = new LineMessageConfig()
            {
                ClientId = Configuration["Line:ClientId"] ?? throw new Exception("Configuation is null."),
                ClientSecret = Configuration["Line:ClientSecret"] ?? throw new Exception("Configuation is null."),
                BaseUrl = Configuration["Line:BaseUrl"] ?? throw new Exception("Configuation is null.")
            };

            AppDbContext ctx = new AppDbContext(Configuration);
            AlertEntity[] alerts = ctx.Alerts.Include(a => a.RegionId).Where(a => !a.AlertTriggered & a.RiskLevel == "High").ToArray();
            if (alerts.Length > 0)
            {
                if (!await messageService.Broadcast(config, alerts))
                {
                    throw new Exception("MessageAPI Error.");
                }

                int txid = ctx.Alerts.Max(a => a.TransactionId) + 1;

                foreach (var alert in alerts)
                {
                    alert.AlertTriggered = true;
                    alert.TransactionId = txid;

                }
                ctx.SaveChanges();

                //cachesss
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(Configuration["ConnectionStrings:RedisConnection"] ?? throw new Exception("Configuation is null."));
                IDatabase db = redis.GetDatabase();
                string cacheString = JsonConvert.SerializeObject(alerts.Select(a => new DisasterRiskResponse(a)).ToArray());
                db.StringSet("lastestAlerts", cacheString, TimeSpan.FromDays(7));
                return Ok(new BaseResponse(true));
            }
            return Ok(new BaseResponse(true, errorMessage: "No Alert Need to Send."));

        }
        catch (Exception ex)
        {
            Log.Error("Error Message: {ErrorMessage}", ex.Message);
            BaseResponse res = new BaseResponse(false, ex.Message);
            return Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("/api/alerts")]
    public IActionResult GetAlerts()
    {
        try
        {
            // Setup Ctx
            AppDbContext ctx = new AppDbContext(Configuration);
            //redis
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(Configuration["ConnectionStrings:RedisConnection"] ?? throw new Exception("Configuation is null."));
            IDatabase db = redis.GetDatabase();
            string cacheString = db.StringGet("lastestAlerts").ToString();
            //string cacheString = "";
            if (!string.IsNullOrEmpty(cacheString))
            {
                return Ok(JsonConvert.DeserializeObject<DisasterRiskResponse[]>(cacheString));
            }
            AlertEntity[] alertEntities = ctx.Alerts.Include(a => a.RegionId).Where(a => a.AlertTriggered & a.TransactionId == ctx.Alerts.Max(a => a.TransactionId)).ToArray();
            return Ok(alertEntities.Select(a => new DisasterRiskResponse(a)).ToArray());
        }
        catch (Exception ex)
        {
            Log.Error("Error Message: {ErrorMessage}", ex.Message);
            BaseResponse res = new BaseResponse(false, ex.Message);
            return Problem(ex.Message);
        }
    }
}
