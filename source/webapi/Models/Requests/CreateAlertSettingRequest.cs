using System.ComponentModel.DataAnnotations;

namespace webapi.Models.Entities;

public class CreateAlertSettingRequest
{
    // Region ID: Identifier for the region.
    public required string RegionId { get; set; }
    // Threshold Score: Risk score threshold that triggers an alert for this disaster type
    public required int ThresholdScore { get; set; }
    // Disaster Type: Type of disaster (must match one monitored by the region).
    public required string DisasterType { get; set; }
}



