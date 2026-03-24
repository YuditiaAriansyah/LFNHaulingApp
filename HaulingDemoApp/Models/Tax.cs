using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("tax_master")]
public class Tax
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string TaxCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string TaxName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string TaxType { get; set; } = "VAT";  // VAT, INCOME_TAX, SALES_TAX

    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxRate { get; set; }  // e.g. 11.00 for 11%

    [MaxLength(10)]
    public string TaxRateType { get; set; } = "PERCENTAGE";  // PERCENTAGE, FIXED

    [Column(TypeName = "decimal(18,2)")]
    public decimal? FixedAmount { get; set; }  // if RateType = FIXED

    [MaxLength(30)]
    public string? CoaCode { get; set; }   // FK to account_master.AccountCode (tax payable)

    [MaxLength(10)]
    public string TaxBase { get; set; } = "EXCLUSIVE";  // EXCLUSIVE, INCLUSIVE

    [MaxLength(20)]
    public string? ApplicableTo { get; set; } = "ALL";  // ALL, GOODS, SERVICES

    [MaxLength(20)]
    public string Status { get; set; } = "ACTIVE";

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class TaxInput
{
    public string TaxCode { get; set; } = "";
    public string TaxName { get; set; } = "";
    public string TaxType { get; set; } = "VAT";
    public decimal TaxRate { get; set; }
    public string TaxRateType { get; set; } = "PERCENTAGE";
    public decimal? FixedAmount { get; set; }
    public string? CoaCode { get; set; }
    public string TaxBase { get; set; } = "EXCLUSIVE";
    public string? ApplicableTo { get; set; }
    public string? Remarks { get; set; }
}
