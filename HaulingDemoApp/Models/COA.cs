using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("coa_master")]
public class COA
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string AccountCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string AccountName { get; set; } = string.Empty;

    [MaxLength(10)]
    public string AccountType { get; set; } = "EXPENSE";  // ASSET, LIABILITY, EQUITY, REVENUE, EXPENSE

    [MaxLength(20)]
    public string AccountCategory { get; set; } = "OPERATIONAL";  // OPERATIONAL, NON_OPERATIONAL

    [MaxLength(5)]
    public int AccountLevel { get; set; } = 5;  // 1=Header, 5=Detail

    [MaxLength(30)]
    public string? ParentAccountCode { get; set; }  // FK to parent account

    [MaxLength(10)]
    public string NormalBalance { get; set; } = "DEBIT";  // DEBIT, CREDIT

    [MaxLength(30)]
    public string? CostCenterRequired { get; set; } = "N";  // Y/N

    [MaxLength(30)]
    public string? SiteCode { get; set; }  // null = global

    [MaxLength(20)]
    public string? TaxCode { get; set; }   // link to tax_master

    [MaxLength(20)]
    public string? Currency { get; set; } = "IDR";

    [MaxLength(50)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "ACTIVE";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class COAInput
{
    public string AccountCode { get; set; } = "";
    public string AccountName { get; set; } = "";
    public string AccountType { get; set; } = "EXPENSE";
    public string AccountCategory { get; set; } = "OPERATIONAL";
    public int AccountLevel { get; set; } = 5;
    public string? ParentAccountCode { get; set; }
    public string NormalBalance { get; set; } = "DEBIT";
    public string? CostCenterRequired { get; set; }
    public string? SiteCode { get; set; }
    public string? TaxCode { get; set; }
    public string Currency { get; set; } = "IDR";
    public string? Description { get; set; }
}
