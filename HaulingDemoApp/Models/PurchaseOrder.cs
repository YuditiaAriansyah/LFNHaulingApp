using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("purchase_orders")]
public class PurchaseOrder
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string PONumber { get; set; } = string.Empty;

    [Required]
    public DateTime PODate { get; set; }

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Vendor { get; set; } = string.Empty;

    [MaxLength(50)]
    public string VendorCode { get; set; } = string.Empty;

    [MaxLength(50)]
    public string PRNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "DRAFT";

    public DateTime? DeliveryDate { get; set; }

    [MaxLength(100)]
    public string DeliveryAddress { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Tax { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(50)]
    public string PaymentTerms { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Remarks { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ApprovedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

[Table("purchase_order_items")]
public class PurchaseOrderItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PurchaseOrderId { get; set; }

    [ForeignKey("PurchaseOrderId")]
    public PurchaseOrder? PurchaseOrder { get; set; }

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
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DeliveredQty { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
