using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("route_master")]
public class Route
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string RouteCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string RouteName { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? SiteCode { get; set; }

    [MaxLength(30)]
    public string? OriginLocation { get; set; }   // Lokasi muat / pit

    [MaxLength(30)]
    public string? DestinationLocation { get; set; }  // Lokasi buang / stockpile

    [MaxLength(10)]
    public decimal? DistanceKm { get; set; }   // Jarak dalam km

    [MaxLength(10)]
    public decimal? TravelTimeMin { get; set; }  // Waktu tempuh dalam menit

    [MaxLength(10)]
    public decimal? HaulCostPerKm { get; set; }  // Biaya hauling per km

    [MaxLength(10)]
    public decimal? FuelConsumptionPerKm { get; set; }  // Konsumsi BBM per km (liter)

    [MaxLength(20)]
    public string? RouteType { get; set; } = "HAUL";  // HAUL, RETURN, OVERBURDEN

    [MaxLength(20)]
    public string? RoadCondition { get; set; }  // GOOD, MEDIUM, POOR

    [MaxLength(20)]
    public string Status { get; set; } = "ACTIVE";

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class RouteInput
{
    public string RouteCode { get; set; } = "";
    public string RouteName { get; set; } = "";
    public string? SiteCode { get; set; }
    public string? OriginLocation { get; set; }
    public string? DestinationLocation { get; set; }
    public decimal? DistanceKm { get; set; }
    public decimal? TravelTimeMin { get; set; }
    public decimal? HaulCostPerKm { get; set; }
    public decimal? FuelConsumptionPerKm { get; set; }
    public string RouteType { get; set; } = "HAUL";
    public string? Remarks { get; set; }
}
