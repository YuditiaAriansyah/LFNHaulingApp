using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("fleet_vehicles")]
public class FleetVehicle
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string UnitNo { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string UnitDescription { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [MaxLength(100)]
    public string MerkType { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    // =============== VEHICLE SPECIFICATIONS ===============
    [MaxLength(20)]
    public string? LicensePlate { get; set; } // Nomor polisi

    [MaxLength(50)]
    public string? ChassisNumber { get; set; } // Nomor rangka

    [MaxLength(50)]
    public string? EngineNumber { get; set; } // Nomor mesin

    [MaxLength(50)]
    public string? VehicleType { get; set; } // OWNED, RENTED, LEASED

    [MaxLength(50)]
    public string? FuelType { get; set; } // DIESEL, SOLAR, DEX, PERTAMINA

    // =============== WEIGHT SPECIFICATIONS ===============
    [Column(TypeName = "decimal(18,4)")]
    public decimal? GrossWeight { get; set; } // Berat kotor (kg) - batas legal

    [Column(TypeName = "decimal(18,4)")]
    public decimal? TareWeight { get; set; } // Berat kosong (kg) - berat kendaraan kosong

    [Column(TypeName = "decimal(18,4)")]
    public decimal? PayloadCapacity { get; set; } // Kapasitas muatan netto (ton) = Gross - Tare

    [Column(TypeName = "decimal(18,4)")]
    public decimal? MaxPayload { get; set; } // Batas maksimal muatan (ton) - dari pabrik

    // =============== FUEL SPECIFICATIONS ===============
    [Column(TypeName = "decimal(18,4)")]
    public decimal? FuelTankCapacity { get; set; } // Kapasitas tangki (liter)

    [Column(TypeName = "decimal(18,4)")]
    public decimal? AvgFuelConsumption { get; set; } // Konsumsi BBM rata-rata (liter/jam) - dari pabrik

    [Column(TypeName = "decimal(18,4)")]
    public decimal? BenchmarkLitrePerKM { get; set; } // Standar konsumsi BBM (liter/km)

    [Column(TypeName = "decimal(18,4)")]
    public decimal? BenchmarkLitrePerHour { get; set; } // Standar konsumsi BBM (liter/jam)

    [MaxLength(50)]
    public string? FuelCardNumber { get; set; } // Nomor kartu BBM

    // =============== TIRE SPECIFICATIONS ===============
    [MaxLength(50)]
    public string? TyreSize { get; set; } // Ukuran ban

    public int TyreQuantity { get; set; } // Jumlah ban

    [Column(TypeName = "decimal(18,4)")]
    public decimal? TyreCostPerUnit { get; set; } // Harga ban per unit

    [Column(TypeName = "decimal(18,4)")]
    public decimal? AvgTyreLifeKM { get; set; } // Rata-rata عمر ban (km)

    // =============== HM / KM ===============
    public DateTime? PemasanganDate { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal HMAwal { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal KMAwal { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal HMakhir { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal KMakhir { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalJam { get; set; } // Total HM usage (HMakhir - HMAwal)

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalKM { get; set; } // Total KM (KMakhir - KMAwal)

    [Column(TypeName = "decimal(18,4)")]
    public decimal FuelRatio { get; set; } // HM/KM ratio

    [Column(TypeName = "decimal(18,4)")]
    public decimal AvgHMHari { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal AvgKMHari { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal HMUsage { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalFuel { get; set; }

    // =============== COST CENTER ===============
    [MaxLength(50)]
    public string? CostCenter { get; set; } // Link ke finance cost center

    [MaxLength(50)]
    public string? RouteCode { get; set; } // Route default

    [MaxLength(50)]
    public string? AssignedDriverId { get; set; } // Driver yang ditugaskan

    // =============== DEPRECIATION ===============
    [Column(TypeName = "decimal(18,4)")]
    public decimal? AcquisitionCost { get; set; } // Harga perolehan

    [Column(TypeName = "decimal(18,4)")]
    public decimal? DepreciationRate { get; set; } // Tarif penyusutan (% per tahun)

    [Column(TypeName = "decimal(18,4)")]
    public decimal? AccumulatedDepreciation { get; set; } // Penyusutan terkumpul

    [Column(TypeName = "decimal(18,4)")]
    public decimal? BookValue { get; set; } // Nilai buku = Acquisition - Accumulated

    [MaxLength(50)]
    public string? UsefulLifeYear { get; set; } // Umur ekonomis (tahun)

    // =============== COMPLIANCE ===============
    public DateTime? InsuranceExpiry { get; set; }
    public DateTime? TaxExpiry { get; set; }
    public DateTime? KIRExpiry { get; set; } // roadworthiness certificate
    public DateTime? STNKExpiry { get; set; } // registration certificate

    // =============== STATUS ===============
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "ACTIVE"; // ACTIVE, STANDBY, MAINTENANCE, BROKEN, RETIRED

    [MaxLength(255)]
    public string Remarks { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// Fleet Summary by Site
public class FleetSummary
{
    public string Site { get; set; } = string.Empty;
    public int TotalUnit { get; set; }
    public int ActiveUnits { get; set; }
    public int MaintenanceUnits { get; set; }
    public int StandbyUnits { get; set; }
    public int BrokenUnits { get; set; }
    public decimal TotalHM { get; set; }
    public decimal TotalKM { get; set; }
    public decimal TotalFuel { get; set; }
    public decimal AvgHMHari { get; set; }
    public decimal AvgKMHari { get; set; }
    public decimal FuelRatio { get; set; }
    public decimal AvgPayloadTon { get; set; }
    public decimal TotalTonCapacity { get; set; }
}

// Fleet Input Model
public class FleetVehicleInput
{
    public string UnitNo { get; set; } = "";
    public string UnitDescription { get; set; } = "";
    public string Site { get; set; } = "";
    public string? MerkType { get; set; }
    public string? Category { get; set; }
    public string? LicensePlate { get; set; }
    public string? ChassisNumber { get; set; }
    public string? EngineNumber { get; set; }
    public string? VehicleType { get; set; }
    public string? FuelType { get; set; }
    public decimal? GrossWeight { get; set; }
    public decimal? TareWeight { get; set; }
    public decimal? PayloadCapacity { get; set; }
    public decimal? MaxPayload { get; set; }
    public decimal? FuelTankCapacity { get; set; }
    public decimal? AvgFuelConsumption { get; set; }
    public decimal? BenchmarkLitrePerKM { get; set; }
    public decimal? BenchmarkLitrePerHour { get; set; }
    public string? FuelCardNumber { get; set; }
    public string? TyreSize { get; set; }
    public int? TyreQuantity { get; set; }
    public decimal? TyreCostPerUnit { get; set; }
    public decimal? AvgTyreLifeKM { get; set; }
    public DateTime? PemasanganDate { get; set; }
    public decimal HMAwal { get; set; }
    public decimal KMAwal { get; set; }
    public decimal HMakhir { get; set; }
    public decimal KMakhir { get; set; }
    public decimal TotalJam { get; set; }
    public decimal TotalKM { get; set; }
    public decimal FuelRatio { get; set; }
    public decimal AvgHMHari { get; set; }
    public decimal AvgKMHari { get; set; }
    public decimal HMUsage { get; set; }
    public decimal TotalFuel { get; set; }
    public string? CostCenter { get; set; }
    public string? RouteCode { get; set; }
    public string? AssignedDriverId { get; set; }
    public decimal? AcquisitionCost { get; set; }
    public decimal? DepreciationRate { get; set; }
    public decimal? AccumulatedDepreciation { get; set; }
    public decimal? BookValue { get; set; }
    public string? UsefulLifeYear { get; set; }
    public DateTime? InsuranceExpiry { get; set; }
    public DateTime? TaxExpiry { get; set; }
    public DateTime? KIRExpiry { get; set; }
    public DateTime? STNKExpiry { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public string? Remarks { get; set; }
}
