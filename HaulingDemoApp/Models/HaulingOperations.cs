using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

// ============ HAUL TRIP / RITASE ============
// Entri inti hauling - mencatat setiap siklus Angkutan (load -> haul -> dump -> return)
[Table("haul_trips")]
public class HaulTrip
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string TripNumber { get; set; } = string.Empty;

    public DateTime TripDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    // Unit
    [Required]
    [MaxLength(50)]
    public string UnitNo { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? DriverId { get; set; }

    [MaxLength(100)]
    public string? DriverName { get; set; }

    // Route
    [Required]
    [MaxLength(50)]
    public string RouteCode { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? RouteName { get; set; }

    // Shift
    [MaxLength(20)]
    public string Shift { get; set; } = "SHIFT_1"; // SHIFT_1, SHIFT_2, SHIFT_3

    // Material
    [MaxLength(50)]
    public string? MaterialType { get; set; } // OVERBURDEN, COAL, ORE, LIMESTONE, etc.

    [MaxLength(50)]
    public string? OriginPit { get; set; }

    [MaxLength(50)]
    public string? DestinationStockpile { get; set; }

    // Weighbridge Data
    [MaxLength(50)]
    public string? WBTicketIn { get; set; } // Inbound weighbridge ticket (empty masuk loader)

    [MaxLength(50)]
    public string? WBTicketOut { get; set; } // Outbound weighbridge ticket (loaded keluar)

    [Column(TypeName = "decimal(18,4)")]
    public decimal? GrossWeight { get; set; } // Berat kotor (kg) - dari weighbridge out

    [Column(TypeName = "decimal(18,4)")]
    public decimal? TareWeight { get; set; } // Berat kosong (kg) - dari weighbridge in

    [Column(TypeName = "decimal(18,4)")]
    public decimal? NetWeight { get; set; } // Berat bersih (kg) = Gross - Tare

    [Column(TypeName = "decimal(18,4)")]
    public decimal? PayloadTon { get; set; } // NetWeight dalam ton

    // Waktu
    public DateTime? StartTime { get; set; } // Waktu mulai trip (mulai bergerak dari loader)
    public DateTime? EndTime { get; set; } // Waktu selesai trip (sampai kembali ke loader)

    [Column(TypeName = "decimal(10,2)")]
    public decimal? CycleTimeMinutes { get; set; } // Total waktu siklus

    [Column(TypeName = "decimal(10,2)")]
    public decimal? LoadingTimeMinutes { get; set; } // Waktu muat

    [Column(TypeName = "decimal(10,2)")]
    public decimal? HaulingTimeMinutes { get; set; } // Waktu angkut (loaded)

    [Column(TypeName = "decimal(10,2)")]
    public decimal? DumpingTimeMinutes { get; set; } // Waktu bongkar

    [Column(TypeName = "decimal(10,2)")]
    public decimal? ReturningTimeMinutes { get; set; } // Waktu kembali (empty)

    // KM & Fuel
    [Column(TypeName = "decimal(18,4)")]
    public decimal? DistanceKM { get; set; } // Jarak tempuh trip (km)

    [Column(TypeName = "decimal(18,4)")]
    public decimal? FuelConsumed { get; set; } // BBM terpakai trip ini (liter)

    [Column(TypeName = "decimal(18,4)")]
    public decimal? FuelRatioLperKM { get; set; } // Liter per KM = FuelConsumed / DistanceKM

    [Column(TypeName = "decimal(18,4)")]
    public decimal? FuelRatioLperTonKM { get; set; } // Liter per Ton-KM = FuelConsumed / (PayloadTon * DistanceKM)

    // HM
    [Column(TypeName = "decimal(18,4)")]
    public decimal? HMStart { get; set; } // Hour Meter saat mulai

    [Column(TypeName = "decimal(18,4)")]
    public decimal? HMEnd { get; set; } // Hour Meter saat selesai

    // Revenue per Trip
    [Column(TypeName = "decimal(18,4)")]
    public decimal? RatePerTon { get; set; } // Tarif per ton (Rp/ton)

    [Column(TypeName = "decimal(18,4)")]
    public decimal? TripRevenue { get; set; } // Total revenue = PayloadTon * RatePerTon

    [Column(TypeName = "decimal(18,4)")]
    public decimal? TripCost { get; set; } // Biaya trip (fuel + driver portion + wear)

    // Status
    [MaxLength(20)]
    public string Status { get; set; } = "COMPLETED"; // DRAFT, COMPLETED, CANCELLED, VALIDATED

    [MaxLength(255)]
    public string? Remarks { get; set; }

    // Validation
    public bool IsWBValidated { get; set; } // Apakah sudah tervalidasi dengan slip weighbridge
    public bool IsRevenuePosted { get; set; } // Apakah sudah posting ke GL

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============ ROUTE MASTER ============
[Table("route_master")]
public class RouteMaster
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string RouteCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string RouteName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Site { get; set; }

    [MaxLength(50)]
    public string? OriginPit { get; set; }

    [MaxLength(50)]
    public string? Destination { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal? DistanceKM { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal? EstimatedCycleTime { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal? GradePercent { get; set; }

    [MaxLength(50)]
    public string? RoadType { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal? EstimatedFuelPerTrip { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal? CostPerKM { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal? CostPerTonKM { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal? RatePerTon { get; set; }

    // P3 additional fields (nullable - may not exist in older data)
    [MaxLength(30)]
    public string? SiteCode { get; set; }

    [MaxLength(30)]
    public string? OriginLocation { get; set; }

    [MaxLength(30)]
    public string? DestinationLocation { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? TravelTimeMin { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? HaulCostPerKm { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? FuelConsumptionPerKm { get; set; }

    [MaxLength(20)]
    public string? RouteType { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "ACTIVE";

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============ WEIGHBRIDGE TICKET ============
[Table("weighbridge_tickets")]
public class WeighbridgeTicket
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string TicketNumber { get; set; } = string.Empty;

    public DateTime TicketDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string TicketType { get; set; } = "IN"; // IN (kosong/masuk), OUT (berat/keluar)

    [Required]
    [MaxLength(50)]
    public string UnitNo { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? DriverName { get; set; }

    [MaxLength(50)]
    public string? DriverBadge { get; set; }

    // Weighbridge readings
    [Column(TypeName = "decimal(18,4)")]
    public decimal? FirstWeight { get; set; } // Berat pertama (kg)

    [Column(TypeName = "decimal(18,4)")]
    public decimal? SecondWeight { get; set; } // Berat kedua (kg)

    [Column(TypeName = "decimal(18,4)")]
    public decimal? NetWeight { get; set; } // Berat netto (kg)

    [MaxLength(50)]
    public string? MaterialType { get; set; } // COAL, OVERBURDEN, ORE, etc.

    [MaxLength(50)]
    public string? OriginPit { get; set; }

    [MaxLength(50)]
    public string? DestinationStockpile { get; set; }

    // Konpensasi Timbangan
    [Column(TypeName = "decimal(18,4)")]
    public decimal? TareCompensation { get; set; } // Compensasi tarra (moisture, etc.)

    [Column(TypeName = "decimal(18,4)")]
    public decimal? FinalNetWeight { get; set; } // Berat netto final setelah kompensasi

    [MaxLength(50)]
    public string? VehicleType { get; set; } // Hauler, DumpTruck, etc.

    [Column(TypeName = "decimal(18,4)")]
    public decimal? AxleLoad { get; set; } // Beban gandar (kg)

    [MaxLength(50)]
    public string? WeighbridgeOperator { get; set; }

    public DateTime? FirstWeighTime { get; set; }
    public DateTime? SecondWeighTime { get; set; }

    // Link ke HaulTrip
    [MaxLength(50)]
    public string? TripNumber { get; set; }

    public bool IsLinked { get; set; } // Apakah sudah linked ke HaulTrip

    [MaxLength(255)]
    public string? Remarks { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "ACTIVE"; // ACTIVE, LINKED, CANCELLED

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============ FUEL CONSUMPTION ANALYSIS ============
[Table("fuel_analyses")]
public class FuelAnalysis
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string PeriodMonth { get; set; } = string.Empty;

    [Required]
    [MaxLength(4)]
    public string PeriodYear { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string UnitNo { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? RouteCode { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalFuelLitres { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalKM { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalTonKM { get; set; } // Total Ton-KM = Sum(PayloadTon * DistanceKM) per trip

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalTon { get; set; } // Total tonase diangkut

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalTrips { get; set; } // Total ritase

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalHours { get; set; } // Total jam operasional

    // Key Ratios
    [Column(TypeName = "decimal(18,4)")]
    public decimal LitrePerKM { get; set; } // TotalFuel / TotalKM

    [Column(TypeName = "decimal(18,4)")]
    public decimal LitrePerTon { get; set; } // TotalFuel / TotalTon

    [Column(TypeName = "decimal(18,4)")]
    public decimal LitrePerTonKM { get; set; } // TotalFuel / TotalTonKM

    [Column(TypeName = "decimal(18,4)")]
    public decimal LitrePerHour { get; set; } // TotalFuel / TotalHours

    [Column(TypeName = "decimal(18,4)")]
    public decimal FuelCost { get; set; } // Total biaya BBM

    [Column(TypeName = "decimal(18,4)")]
    public decimal FuelCostPerTon { get; set; } // FuelCost / TotalTon

    [Column(TypeName = "decimal(18,4)")]
    public decimal? BenchmarkLitrePerKM { get; set; } // Standar pabrik (liter/km)

    [Column(TypeName = "decimal(18,4)")]
    public decimal? VariancePercent { get; set; } // Variansi dari benchmark (%)

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ============ UNIT COST TRACKING ============
[Table("unit_cost_trackings")]
public class UnitCostTracking
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string PeriodMonth { get; set; } = string.Empty;

    [Required]
    [MaxLength(4)]
    public string PeriodYear { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string UnitNo { get; set; } = string.Empty;

    [MaxLength(50)]
    [NotMapped]
    public string? CostCenter { get; set; }

    [MaxLength(50)]
    [NotMapped]
    public string? Category { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal FuelCost { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal MaintenanceCost { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal DriverCost { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal DepreciationCost { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TyreCost { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal OtherCost { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalCost { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalTrips { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalTon { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalKM { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalHours { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal CostPerTrip { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal CostPerTon { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal CostPerKM { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal CostPerHour { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ============ DRIVER PRODUCTIVITY ============
[Table("driver_productivities")]
public class DriverProductivity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string PeriodMonth { get; set; } = string.Empty;

    [Required]
    [MaxLength(4)]
    public string PeriodYear { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string DriverId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string DriverName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string UnitNo { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalTrips { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TargetTrips { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal AchievementPercent { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalTon { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalKM { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalHours { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalFuelLitres { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal FuelEfficiency { get; set; } // Litre per KM

    [Column(TypeName = "decimal(18,4)")]
    public int WorkingDays { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal LateCount { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal AccidentCount { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal ViolationCount { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal RitaseAllowance { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal IncentiveAmount { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal DeductionAmount { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalPayable { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ============ INPUT MODELS ============
public class HaulTripInput
{
    public string? TripNumber { get; set; }
    public DateTime? TripDate { get; set; }
    public string Site { get; set; } = "";
    public string UnitNo { get; set; } = "";
    public string? DriverId { get; set; }
    public string? DriverName { get; set; }
    public string RouteCode { get; set; } = "";
    public string? RouteName { get; set; }
    public string? Shift { get; set; }
    public string? MaterialType { get; set; }
    public string? OriginPit { get; set; }
    public string? DestinationStockpile { get; set; }
    public string? WBTicketIn { get; set; }
    public string? WBTicketOut { get; set; }
    public decimal? GrossWeight { get; set; }
    public decimal? TareWeight { get; set; }
    public decimal? NetWeight { get; set; }
    public decimal? PayloadTon { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal? CycleTimeMinutes { get; set; }
    public decimal? LoadingTimeMinutes { get; set; }
    public decimal? HaulingTimeMinutes { get; set; }
    public decimal? DumpingTimeMinutes { get; set; }
    public decimal? ReturningTimeMinutes { get; set; }
    public decimal? DistanceKM { get; set; }
    public decimal? FuelConsumed { get; set; }
    public decimal? HMStart { get; set; }
    public decimal? HMEnd { get; set; }
    public decimal? RatePerTon { get; set; }
    public decimal? TripRevenue { get; set; }
    public decimal? TripCost { get; set; }
    public string? Remarks { get; set; }
}

public class RouteMasterInput
{
    public string? RouteCode { get; set; }
    public string RouteName { get; set; } = "";
    public string Site { get; set; } = "";
    public string? OriginPit { get; set; }
    public string? Destination { get; set; }
    public decimal? DistanceKM { get; set; }
    public decimal? EstimatedCycleTime { get; set; }
    public decimal? GradePercent { get; set; }
    public string? RoadType { get; set; }
    public decimal? EstimatedFuelPerTrip { get; set; }
    public decimal? CostPerKM { get; set; }
    public decimal? CostPerTonKM { get; set; }
    public decimal? RatePerTon { get; set; }
    public string? Remarks { get; set; }
}

public class WeighbridgeTicketInput
{
    public string? TicketNumber { get; set; }
    public DateTime? TicketDate { get; set; }
    public string Site { get; set; } = "";
    public string TicketType { get; set; } = "IN";
    public string UnitNo { get; set; } = "";
    public string? DriverName { get; set; }
    public string? DriverBadge { get; set; }
    public decimal? FirstWeight { get; set; }
    public decimal? SecondWeight { get; set; }
    public decimal? NetWeight { get; set; }
    public string? MaterialType { get; set; }
    public string? OriginPit { get; set; }
    public string? DestinationStockpile { get; set; }
    public decimal? TareCompensation { get; set; }
    public decimal? FinalNetWeight { get; set; }
    public string? VehicleType { get; set; }
    public decimal? AxleLoad { get; set; }
    public string? WeighbridgeOperator { get; set; }
    public DateTime? FirstWeighTime { get; set; }
    public DateTime? SecondWeighTime { get; set; }
    public string? TripNumber { get; set; }
    public string? Remarks { get; set; }
}

public class FuelAnalysisInput
{
    public string PeriodMonth { get; set; } = "";
    public string PeriodYear { get; set; } = "";
    public string Site { get; set; } = "";
    public string UnitNo { get; set; } = "";
    public string? RouteCode { get; set; }
    public decimal? BenchmarkLitrePerKM { get; set; }
}

public class UnitCostTrackingInput
{
    public string PeriodMonth { get; set; } = "";
    public string PeriodYear { get; set; } = "";
    public string Site { get; set; } = "";
    public string UnitNo { get; set; } = "";
    public string? CostCenter { get; set; }
    public string? Category { get; set; }
}

public class DriverProductivityInput
{
    public string PeriodMonth { get; set; } = "";
    public string PeriodYear { get; set; } = "";
    public string Site { get; set; } = "";
    public string DriverId { get; set; } = "";
    public string? DriverName { get; set; }
    public string UnitNo { get; set; } = "";
    public decimal? TargetTrips { get; set; }
}

// ============ SUMMARY MODELS ============
public class HaulTripSummary
{
    public string Site { get; set; } = "";
    public string Period { get; set; } = "";
    public int TotalTrips { get; set; }
    public decimal TotalTon { get; set; }
    public decimal TotalKM { get; set; }
    public decimal TotalHours { get; set; }
    public decimal TotalFuel { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCost { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public decimal AvgCycleTimeMinutes { get; set; }
    public decimal AvgPayloadTon { get; set; }
    public decimal AvgFuelPerTrip { get; set; }
    public decimal AvgTripsPerUnit { get; set; }
    public decimal AvgTonPerUnit { get; set; }
}

public class RouteEfficiencySummary
{
    public string RouteCode { get; set; } = "";
    public string RouteName { get; set; } = "";
    public string Site { get; set; } = "";
    public int TotalTrips { get; set; }
    public decimal TotalTon { get; set; }
    public decimal TotalTonKM { get; set; }
    public decimal AvgPayloadTon { get; set; }
    public decimal AvgCycleTimeMinutes { get; set; }
    public decimal AvgFuelPerTrip { get; set; }
    public decimal LitrePerTonKM { get; set; }
    public decimal ActualCostPerTonKM { get; set; }
    public decimal RouteRatePerTon { get; set; }
    public decimal RouteRevenue { get; set; }
    public decimal RouteProfit { get; set; }
}

public class FuelEfficiencySummary
{
    public string Site { get; set; } = "";
    public int TotalUnits { get; set; }
    public decimal TotalFuelLitres { get; set; }
    public decimal TotalKM { get; set; }
    public decimal TotalTon { get; set; }
    public decimal TotalTonKM { get; set; }
    public decimal TotalTrips { get; set; }
    public decimal AvgLitrePerKM { get; set; }
    public decimal AvgLitrePerTon { get; set; }
    public decimal AvgLitrePerTonKM { get; set; }
    public decimal AvgLitrePerHour { get; set; }
    public decimal TotalFuelCost { get; set; }
    public decimal FuelCostPerTon { get; set; }
    public int UnderPerformers { get; set; } // units with variance > 10%
    public int OverPerformers { get; set; } // units with variance < -10%
}

public class WeighbridgeReconciliation
{
    public int TotalTickets { get; set; }
    public int LinkedTrips { get; set; }
    public int UnlinkedTickets { get; set; }
    public decimal LinkedTon { get; set; }
    public decimal UnlinkedTon { get; set; }
    public decimal VarianceTon { get; set; } // selisih dari weighbridge vs input
    public decimal VariancePercent { get; set; }
    public List<WeighbridgeTicket> UnlinkedList { get; set; } = new();
}
