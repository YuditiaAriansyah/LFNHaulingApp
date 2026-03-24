using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("purchase_requests")]
public class PurchaseRequest
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string PRNumber { get; set; } = string.Empty;

    [Required]
    public DateTime PRDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [MaxLength(100)]
    public string RequestedBy { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ApprovedBy { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "DRAFT";

    [MaxLength(255)]
    public string Remarks { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

[Table("purchase_request_items")]
public class PurchaseRequestItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PurchaseRequestId { get; set; }

    [ForeignKey("PurchaseRequestId")]
    public PurchaseRequest? PurchaseRequest { get; set; }

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
    public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal EstimatedPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    [MaxLength(100)]
    public string Purpose { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Priority { get; set; } = "NORMAL";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
