using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("material_master")]
public class Material
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string MaterialCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string MaterialName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? MaterialGroup { get; set; }   // FUEL, TYRE, PART, SERVICE, OTHERS

    [MaxLength(20)]
    public string MaterialType { get; set; } = "ITEM";  // ITEM, SERVICE

    [MaxLength(20)]
    public string? UomCode { get; set; }   // FK to uom_master.UomCode

    [MaxLength(50)]
    public string? Brand { get; set; }

    [MaxLength(50)]
    public string? Spec { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? UnitPrice { get; set; }

    [MaxLength(20)]
    public string Currency { get; set; } = "IDR";

    [MaxLength(20)]
    public string? SiteCode { get; set; }   // FK to site_master.SiteCode

    [MaxLength(50)]
    public string Status { get; set; } = "ACTIVE";

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class MaterialInput
{
    public string MaterialCode { get; set; } = "";
    public string MaterialName { get; set; } = "";
    public string? MaterialGroup { get; set; }
    public string MaterialType { get; set; } = "ITEM";
    public string? UomCode { get; set; }
    public string? Brand { get; set; }
    public string? Spec { get; set; }
    public decimal? UnitPrice { get; set; }
    public string Currency { get; set; } = "IDR";
    public string? SiteCode { get; set; }
    public string? Remarks { get; set; }
}
