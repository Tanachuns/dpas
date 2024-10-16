using System.ComponentModel.DataAnnotations;

namespace webapi.Models.Entities;

public class AlertSettingEntity : BaseEntity
{
    public int Id { get; set; }
    //Region ID: Identifier for the region.
    public int RegionID { get; set; }
    //Disaster Type: Type of disaster (must match one monitored by the region).
    public string? DisasterType { get; set; }
    //Threshold Score: Risk score threshold that triggers an alert for this disaster type.
    public int ThresholdScore { get; set; }



}



