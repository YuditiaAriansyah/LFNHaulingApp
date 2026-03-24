using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

// ============ WORK ORDER ============
[Table("work_orders")]
public class WorkOrder
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string WONumber { get; set; } = string.Empty;

    public DateTime WODate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string UnitNo { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? MerkType { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; }

    [Required]
    [MaxLength(50)]
    public string WOType { get; set; } = "PREVENTIVE"; // PREVENTIVE, CORRECTIVE, BREAKDOWN

    [Required]
    [MaxLength(50)]
    public string Priority { get; set; } = "MEDIUM"; // LOW, MEDIUM, HIGH, CRITICAL

    [MaxLength(255)]
    public string? Problem { get; set; }

    [MaxLength(255)]
    public string? Cause { get; set; }

    [MaxLength(255)]
    public string? ActionTaken { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "OPEN"; // OPEN, IN_PROGRESS, COMPLETED, CANCELLED

    public DateTime? ScheduledDate { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? EstimatedCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ActualCost { get; set; }

    [MaxLength(100)]
    public string? AssignedTo { get; set; }

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============ PREVENTIVE MAINTENANCE ============
[Table("preventive_maintenance")]
public class PreventiveMaintenance
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string PMNumber { get; set; } = string.Empty;

    public DateTime PMDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string UnitNo { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? MerkType { get; set; }

    [Required]
    [MaxLength(50)]
    public string PMType { get; set; } = "DAILY"; // DAILY, WEEKLY, MONTHLY, QUARTERLY, YEARLY

    [MaxLength(255)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "SCHEDULED"; // SCHEDULED, IN_PROGRESS, COMPLETED, OVERDUE

    public DateTime? ScheduledDate { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? HMValue { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? NextHMValue { get; set; }

    [MaxLength(100)]
    public string? AssignedTo { get; set; }

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============ CORRECTIVE MAINTENANCE ============
[Table("corrective_maintenance")]
public class CorrectiveMaintenance
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string CMNumber { get; set; } = string.Empty;

    public DateTime CMDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string UnitNo { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? MerkType { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; }

    [Required]
    [MaxLength(50)]
    public string CMType { get; set; } = "CORRECTIVE"; // CORRECTIVE, BREAKDOWN, EMERGENCY

    [Required]
    [MaxLength(50)]
    public string Priority { get; set; } = "MEDIUM"; // LOW, MEDIUM, HIGH, CRITICAL

    [Required]
    [MaxLength(255)]
    public string Problem { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? RootCause { get; set; }

    [MaxLength(255)]
    public string? Solution { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "REPORTED"; // REPORTED, DIAGNOSING, WAITING_PARTS, IN_REPAIR, COMPLETED

    public DateTime? BreakdownStart { get; set; }

    public DateTime? BreakdownEnd { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DowntimeHours { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? RepairCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PartsCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? LaborCost { get; set; }

    [MaxLength(100)]
    public string? ReportedBy { get; set; }

    [MaxLength(100)]
    public string? AssignedTo { get; set; }

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============ INPUT MODELS ============
public class WorkOrderInput
{
    public string? WONumber { get; set; }
    public DateTime? WODate { get; set; }
    public string Site { get; set; } = "";
    public string UnitNo { get; set; } = "";
    public string? MerkType { get; set; }
    public string? Category { get; set; }
    public string WOType { get; set; } = "PREVENTIVE";
    public string Priority { get; set; } = "MEDIUM";
    public string? Problem { get; set; }
    public string? Cause { get; set; }
    public string? ActionTaken { get; set; }
    public string Status { get; set; } = "OPEN";
    public DateTime? ScheduledDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
}

public class PMInput
{
    public string? PMNumber { get; set; }
    public DateTime? PMDate { get; set; }
    public string Site { get; set; } = "";
    public string UnitNo { get; set; } = "";
    public string? MerkType { get; set; }
    public string PMType { get; set; } = "DAILY";
    public string? Description { get; set; }
    public string Status { get; set; } = "SCHEDULED";
    public DateTime? ScheduledDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? HMValue { get; set; }
    public decimal? NextHMValue { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
}

public class CMInput
{
    public string? CMNumber { get; set; }
    public DateTime? CMDate { get; set; }
    public string Site { get; set; } = "";
    public string UnitNo { get; set; } = "";
    public string? MerkType { get; set; }
    public string? Category { get; set; }
    public string CMType { get; set; } = "CORRECTIVE";
    public string Priority { get; set; } = "MEDIUM";
    public string Problem { get; set; } = "";
    public string? RootCause { get; set; }
    public string? Solution { get; set; }
    public string Status { get; set; } = "REPORTED";
    public DateTime? BreakdownStart { get; set; }
    public DateTime? BreakdownEnd { get; set; }
    public decimal? DowntimeHours { get; set; }
    public decimal? RepairCost { get; set; }
    public decimal? PartsCost { get; set; }
    public decimal? LaborCost { get; set; }
    public string? ReportedBy { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
}

// ============ SUMMARY MODELS ============
public class RMSummary
{
    public int TotalWorkOrders { get; set; }
    public int OpenWorkOrders { get; set; }
    public int InProgressWorkOrders { get; set; }
    public int CompletedWorkOrders { get; set; }
    public int TotalPM { get; set; }
    public int ScheduledPM { get; set; }
    public int CompletedPM { get; set; }
    public int OverduePM { get; set; }
    public int TotalCorrective { get; set; }
    public int BreakdownCount { get; set; }
    public decimal TotalDowntimeHours { get; set; }
    public decimal TotalRepairCost { get; set; }
}
