using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("approval_workflow")]
public class ApprovalWorkflow
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string WorkflowName { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string ModuleType { get; set; } = string.Empty;  // PR, PO, GR, FUEL_USAGE, RM

    [MaxLength(30)]
    public string? SiteCode { get; set; }   // null = global

    [MaxLength(30)]
    public string? CostCenter { get; set; }

    public int ApprovalOrder { get; set; } = 1;  // step sequence

    [MaxLength(30)]
    public string ApproverRole { get; set; } = string.Empty;  // ADMIN, MANAGER, USER

    [MaxLength(100)]
    public string? ApproverName { get; set; }

    [MaxLength(20)]
    public string ApprovalLevel { get; set; } = "REQUIRED";  // REQUIRED, OPTIONAL, SKIP

    [Column(TypeName = "decimal(18,2)")]
    public decimal? MinAmount { get; set; }   // threshold: approval needed if >= this amount

    [Column(TypeName = "decimal(18,2)")]
    public decimal? MaxAmount { get; set; }   // threshold: approval needed if <= this amount

    [MaxLength(50)]
    public string Status { get; set; } = "ACTIVE";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class ApprovalWorkflowInput
{
    public string WorkflowName { get; set; } = "";
    public string ModuleType { get; set; } = "";
    public string? SiteCode { get; set; }
    public string? CostCenter { get; set; }
    public int ApprovalOrder { get; set; } = 1;
    public string ApproverRole { get; set; } = "";
    public string? ApproverName { get; set; }
    public string ApprovalLevel { get; set; } = "REQUIRED";
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}
