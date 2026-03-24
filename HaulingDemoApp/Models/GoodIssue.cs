using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("good_issues")]
public class GoodIssue
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string GINumber { get; set; } = string.Empty;

    [Required]
    public DateTime GIDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [MaxLength(50)]
    public string RequestNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "ISSUED";

    [MaxLength(100)]
    public string IssuedBy { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ReceivedBy { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ApprovedBy { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Remarks { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

[Table("good_issue_items")]
public class GoodIssueItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int GoodIssueId { get; set; }

    [ForeignKey("GoodIssueId")]
    public GoodIssue? GoodIssue { get; set; }

    [Required]
    [MaxLength(50)]
    public string PartNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal RequestedQty { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal IssuedQty { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal StockBefore { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal StockAfter { get; set; }

    [MaxLength(100)]
    public string Purpose { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Remarks { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
