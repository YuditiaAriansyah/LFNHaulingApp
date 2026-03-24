using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HaulingDemoApp.Models;

// ============ CHART OF ACCOUNTS ============
[Table("chart_of_accounts")]
public class ChartOfAccount
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string AccountCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string AccountName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string AccountType { get; set; } = string.Empty;
    // ASSET, LIABILITY, EQUITY, REVENUE, EXPENSE

    [MaxLength(20)]
    public string? ParentAccountCode { get; set; }

    [MaxLength(10)]
    public string NormalBalance { get; set; } = "DEBIT";
    // DEBIT, CREDIT

    [MaxLength(50)]
    public string? CostCenterRequired { get; set; } // YES, NO

    [MaxLength(50)]
    public string? TaxCode { get; set; }

    [MaxLength(50)]
    public string Currency { get; set; } = "IDR";

    [Column(TypeName = "decimal(18,2)")]
    public decimal OpeningBalance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentBalance { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(100)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============ BUDGET ============
[Table("budgets")]
public class Budget
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string BudgetNumber { get; set; } = string.Empty;

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
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Division { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string AccountCode { get; set; } = string.Empty;

    [MaxLength(200)]
    public string AccountName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal PlannedAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ActualAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CommittedAmount { get; set; } // PO/GR in progress

    [Column(TypeName = "decimal(18,2)")]
    public decimal AvailableBudget { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal UtilizationPercent { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "DRAFT";
    // DRAFT, SUBMITTED, APPROVED, CLOSED

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime? ApprovedDate { get; set; }

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============ JOURNAL ENTRY ============
[Table("journal_entries")]
public class JournalEntry
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string EntryNumber { get; set; } = string.Empty;

    public DateTime EntryDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(10)]
    public string PeriodMonth { get; set; } = string.Empty;

    [Required]
    [MaxLength(4)]
    public string PeriodYear { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string EntryType { get; set; } = string.Empty;
    // AUTO, MANUAL, ADJUSTMENT, OPENING, CLOSING

    [MaxLength(100)]
    public string SourceModule { get; set; } = string.Empty;
    // PO, GR, GI, PAYROLL, FUEL, MAINTENANCE, MANUAL

    [MaxLength(50)]
    public string? SourceId { get; set; }

    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "DRAFT";
    // DRAFT, POSTED, CANCELLED

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalDebit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalCredit { get; set; }

    [MaxLength(100)]
    public string? PostedBy { get; set; }

    public DateTime? PostedAt { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<JournalLine>? JournalLines { get; set; }
}

// ============ JOURNAL LINE ============
[Table("journal_lines")]
public class JournalLine
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int JournalEntryId { get; set; }

    [ForeignKey("JournalEntryId")]
    [JsonIgnore]
    public JournalEntry? JournalEntry { get; set; }

    [Required]
    [MaxLength(50)]
    public string AccountCode { get; set; } = string.Empty;

    [MaxLength(200)]
    public string AccountName { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string DC { get; set; } = "D";
    // D = Debit, C = Credit

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(100)]
    public string? CostCenter { get; set; }

    [MaxLength(100)]
    public string? Site { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ============ PRODUCTION DATA ============
[Table("production_data")]
public class ProductionData
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
    [MaxLength(100)]
    public string Division { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalTonase { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalOperatingHours { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalOverburden { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal HaulingDistance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CostPerTon { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal FuelCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MaintenanceCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LaborCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OtherCost { get; set; }

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============ INPUT MODELS ============
public class BudgetInput
{
    public string? BudgetNumber { get; set; }
    public string PeriodMonth { get; set; } = "";
    public string PeriodYear { get; set; } = "";
    public string Site { get; set; } = "";
    public string Department { get; set; } = "";
    public string Division { get; set; } = "";
    public string AccountCode { get; set; } = "";
    public string? AccountName { get; set; }
    public decimal PlannedAmount { get; set; }
    public string? Remarks { get; set; }
}

public class JournalEntryInput
{
    public string? EntryNumber { get; set; }
    public DateTime? EntryDate { get; set; }
    public string EntryType { get; set; } = "MANUAL";
    public string? SourceModule { get; set; }
    public string? SourceId { get; set; }
    public string Description { get; set; } = "";
    public List<JournalLineInput>? Lines { get; set; }
}

public class JournalLineInput
{
    public string AccountCode { get; set; } = "";
    public string? AccountName { get; set; }
    public string DC { get; set; } = "D";
    public decimal Amount { get; set; }
    public string? CostCenter { get; set; }
    public string? Site { get; set; }
    public string? Description { get; set; }
}

public class ProductionDataInput
{
    public string PeriodMonth { get; set; } = "";
    public string PeriodYear { get; set; } = "";
    public string Site { get; set; } = "";
    public string Division { get; set; } = "";
    public decimal TotalTonase { get; set; }
    public decimal TotalOperatingHours { get; set; }
    public decimal TotalOverburden { get; set; }
    public decimal HaulingDistance { get; set; }
    public decimal FuelCost { get; set; }
    public decimal MaintenanceCost { get; set; }
    public decimal LaborCost { get; set; }
    public decimal OtherCost { get; set; }
    public string? Remarks { get; set; }
}

// ============ SUMMARY MODELS ============
public class BudgetSummary
{
    public int TotalBudgets { get; set; }
    public int ActiveBudgets { get; set; }
    public decimal TotalPlanned { get; set; }
    public decimal TotalActual { get; set; }
    public decimal TotalCommitted { get; set; }
    public decimal OverallUtilization { get; set; }
    public List<BudgetAlert> Alerts { get; set; } = new();
}

public class BudgetAlert
{
    public string BudgetNumber { get; set; } = "";
    public string AccountCode { get; set; } = "";
    public string AccountName { get; set; } = "";
    public decimal PlannedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public decimal UtilizationPercent { get; set; }
    public string AlertLevel { get; set; } = "";
    // WARNING (>75%), CRITICAL (>90%), EXCEEDED (>100%)
}

public class GLSummary
{
    public int TotalEntries { get; set; }
    public int PostedEntries { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public List<AccountBalance> TopAccounts { get; set; } = new();
}

public class AccountBalance
{
    public string AccountCode { get; set; } = "";
    public string AccountName { get; set; } = "";
    public string AccountType { get; set; } = "";
    public decimal Balance { get; set; }
}

public class CostPerTonSummary
{
    public string Period { get; set; } = "";
    public string Site { get; set; } = "";
    public decimal TotalTonase { get; set; }
    public decimal TotalCost { get; set; }
    public decimal CostPerTon { get; set; }
    public decimal FuelCost { get; set; }
    public decimal MaintenanceCost { get; set; }
    public decimal LaborCost { get; set; }
    public decimal OtherCost { get; set; }
    public decimal FuelCostPerTon { get; set; }
    public decimal MaintCostPerTon { get; set; }
}
