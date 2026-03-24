using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("vehicle_master")]
public class Vehicle
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string VehicleCode { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? PoliceNumber { get; set; }   // Nomor polisi

    [Required]
    [MaxLength(20)]
    public string VehicleType { get; set; } = string.Empty;  // HAULING, SUPPORT, FUEL_TANKER

    [MaxLength(30)]
    public string? Brand { get; set; }   // HINO, SCANIA, FUSO, dll

    [MaxLength(30)]
    public string? Model { get; set; }

    [MaxLength(20)]
    public string? FuelType { get; set; } = "DIESEL";  // DIESEL, SOLAR

    [MaxLength(30)]
    public string? SiteCode { get; set; }

    [MaxLength(30)]
    public string? CostCenter { get; set; }

    [MaxLength(20)]
    public int? CapacityVolume { get; set; }   // in cubic meters

    [MaxLength(20)]
    public int? CapacityWeight { get; set; }   // in tons

    [MaxLength(20)]
    public int? YearMade { get; set; }

    [MaxLength(30)]
    public string ChassisNumber { get; set; } = string.Empty;

    [MaxLength(30)]
    public string MachineNumber { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Status { get; set; } = "ACTIVE";  // ACTIVE, MAINTENANCE, IDLE, SCRAP

    [MaxLength(100)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class VehicleInput
{
    public string VehicleCode { get; set; } = "";
    public string? PoliceNumber { get; set; }
    public string VehicleType { get; set; } = "HAULING";
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? FuelType { get; set; }
    public string? SiteCode { get; set; }
    public string? CostCenter { get; set; }
    public int? CapacityVolume { get; set; }
    public int? CapacityWeight { get; set; }
    public int? YearMade { get; set; }
    public string ChassisNumber { get; set; } = "";
    public string MachineNumber { get; set; } = "";
    public string? Remarks { get; set; }
}
