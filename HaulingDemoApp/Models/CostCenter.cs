using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("cost_center_master")]
public class CostCenter
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string CostCenterCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string CostCenterName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string SiteCode { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? DepartmentCode { get; set; }

    [MaxLength(30)]
    public string Type { get; set; } = "OPERATIONAL"; // OPERATIONAL, ADMIN, SALES, PROJECT

    [MaxLength(30)]
    public string? ParentCode { get; set; }

    [MaxLength(50)]
    public string Level { get; set; } = "1"; // 1, 2, 3 (hierarchy level)

    [Column(TypeName = "decimal(18,2)")]
    public decimal? AllocatedBudget { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? UsedBudget { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? CommittedBudget { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? AvailableBudget { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "ACTIVE"; // ACTIVE, INACTIVE

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class CostCenterInput
{
    public string CostCenterCode { get; set; } = "";
    public string CostCenterName { get; set; } = "";
    public string? SiteCode { get; set; }
    public string? DepartmentCode { get; set; }
    public string Type { get; set; } = "OPERATIONAL";
    public string? ParentCode { get; set; }
    public string Level { get; set; } = "1";
    public decimal? AllocatedBudget { get; set; }
    public decimal? UsedBudget { get; set; }
    public decimal? CommittedBudget { get; set; }
    public string? Remarks { get; set; }
}
