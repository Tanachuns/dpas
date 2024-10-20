using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using webapi.Models.Entities;

public class AlertEntity : BaseEntity
{
    [JsonIgnore]
    public int Id { get; set; }
    //RegionId
    public required RegionEntity RegionId { get; set; }
    //DisasterType
    public string? DisasterType { get; set; }
    //RiskScore
    public decimal RiskScore { get; set; }
    //RiskLevel
    public string? RiskLevel { get; set; } //High - Medium - Low
                                           //AlertTriggered
    public bool AlertTriggered { get; set; } = false;
}