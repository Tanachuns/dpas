using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace webapi.Models.Entities;

public class RegionEntity : BaseEntity
{
    [Key]
    //Region ID: Unique identifier for each region.
    public required string RegionId { get; set; }
    //Location Coordinates: Latitude and longitude of the region.
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    //Disaster Types: List of disaster types to monitor (e.g., flood, wildfire, earthquake).
    public string[]? DisasterTypes { get; set; }
}


