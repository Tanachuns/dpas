

using System.Diagnostics.Eventing.Reader;

namespace webapi.Models
{
    public class DisasterRiskResponse
    {
        //RegionId
        public required string RegionId { get; set; }
        //DisasterType
        public string? DisasterType { get; set; }
        //RiskScore
        public int RiskScore { get; set; }
        //RiskLevel
        public string? RiskLevel { get; set; } //High - Medium - Low
        //AlertTriggered
        public bool AlertTriggered { get; set; }
    }
}