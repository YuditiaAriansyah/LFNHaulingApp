using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("good_receipts")]
public class GoodReceipt
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string GRNumber { get; set; } = string.Empty;

    [Required]
    public DateTime GRDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Vendor { get; set; } = string.Empty;

    [MaxLength(50)]
    public string PONumber { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; set; } = "RECEIVED";

    [MaxLength(100)]
    public string ReceivedBy { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ApprovedBy { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Remarks { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

[Table("good_receipt_items")]
public class GoodReceiptItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int GoodReceiptId { get; set; }

    [ForeignKey("GoodReceiptId")]
    public GoodReceipt? GoodReceipt { get; set; }

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
    public decimal OrderedQty { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ReceivedQty { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AcceptedQty { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RejectedQty { get; set; }

    [MaxLength(100)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Remarks { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
