using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("vendors")]
public class Vendor
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string VendorCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string VendorName { get; set; } = string.Empty;

    [MaxLength(10)]
    public string VendorType { get; set; } = "SUPPLIER";

    [MaxLength(255)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(50)]
    public string? Province { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(50)]
    public string? Country { get; set; } = "Indonesia";

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? Fax { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(100)]
    public string? ContactPerson { get; set; }

    [MaxLength(50)]
    public string? ContactPhone { get; set; }

    [MaxLength(50)]
    public string? TaxId { get; set; }   // NPWP

    [MaxLength(30)]
    public string? NIB { get; set; }    // NIB / Numpang Importir

    [MaxLength(50)]
    public string? BankName { get; set; }

    [MaxLength(50)]
    public string? BankAccountName { get; set; }

    [MaxLength(50)]
    public string? BankAccountNumber { get; set; }

    [MaxLength(50)]
    public string? BankBranch { get; set; }

    [MaxLength(50)]
    public string? PaymentTerms { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "ACTIVE";

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPurchases { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OutstandingBalance { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
