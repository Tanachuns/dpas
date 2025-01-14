using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace webapi.Models.Entities;

public class AlertSettingEntity : BaseEntity
{

    [JsonIgnore]
    public int Id { get; set; }
    //Region ID: Identifier for the region.
    public required RegionEntity RegionID { get; set; }
    //Disaster Type: Type of disaster (must match one monitored by the region).
    public required string DisasterType { get; set; }
    //Threshold Score: Risk score threshold that triggers an alert for this disaster type.
    public required int ThresholdScore { get; set; }



}



