

using System.Diagnostics.Eventing.Reader;

namespace webapi.Models
{
    public class DisasterRiskResponse
    {
        //RegionId
        public string RegionId { get; set; }
        //DisasterType
        public string? DisasterType { get; set; }
        //RiskScore
        public decimal RiskScore { get; set; }
        //RiskLevel
        public string? RiskLevel { get; set; } //High - Medium - Low
        //AlertTriggered
        public bool AlertTriggered { get; set; }
        public DisasterRiskResponse() { }
        public DisasterRiskResponse(AlertEntity alert)
        {
            RegionId = alert.RegionId.RegionId;
            DisasterType = alert.DisasterType;
            RiskScore = alert.RiskScore;
            RiskLevel = alert.RiskLevel;
            AlertTriggered = alert.AlertTriggered;
        }

    }

}