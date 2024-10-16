using System.ComponentModel.DataAnnotations;

namespace webapi.Models.Entities;

public class CreateRegionRequest
{
    public required string RegionId { get; set; }
    //Location Coordinates: Latitude and longitude of the region.
    public required Coordinate LocationCoordination { get; set; }
    //Disaster Types: List of disaster types to monitor (e.g., flood, wildfire, earthquake).
    public string[]? DisasterTypes { get; set; }
}



