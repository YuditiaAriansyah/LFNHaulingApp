using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("uom_master")]
public class UnitOfMeasure
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string UomCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string UomName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string UomType { get; set; } = "VOLUME";  // VOLUME, WEIGHT, LENGTH, COUNT, TIME, CURRENCY

    [MaxLength(10)]
    public string? BaseUomCode { get; set; }   // conversion base

    [Column(TypeName = "decimal(18,6)")]
    public decimal? ConversionFactor { get; set; }

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class UnitOfMeasureInput
{
    public string UomCode { get; set; } = "";
    public string UomName { get; set; } = "";
    public string UomType { get; set; } = "VOLUME";
    public string? BaseUomCode { get; set; }
    public decimal? ConversionFactor { get; set; }
    public string? Remarks { get; set; }
}
