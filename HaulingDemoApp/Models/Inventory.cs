using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("inventories")]
public class Inventory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string PartNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string MaterialDescription { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Location { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal MinStock { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MaxStock { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Stock { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal QtyMinAlert { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal StockValue { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LastPOPrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
