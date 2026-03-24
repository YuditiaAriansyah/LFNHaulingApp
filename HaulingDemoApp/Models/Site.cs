using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("site_master")]
public class Site
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string SiteCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string SiteName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Region { get; set; }

    [MaxLength(255)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? City { get; set; }

    [MaxLength(50)]
    public string? Province { get; set; }

    [MaxLength(50)]
    public string SiteType { get; set; } = "MINING"; // MINING, QUARRY, PORT, WAREHOUSE

    [MaxLength(20)]
    public string Currency { get; set; } = "IDR";

    [MaxLength(20)]
    public string TimeZone { get; set; } = "Asia/Makassar"; // WITA, WIB, WIT

    [MaxLength(50)]
    public string Status { get; set; } = "ACTIVE"; // ACTIVE, INACTIVE

    [MaxLength(100)]
    public string? ContactPerson { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class SiteInput
{
    public string SiteCode { get; set; } = "";
    public string SiteName { get; set; } = "";
    public string? Region { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string SiteType { get; set; } = "MINING";
    public string Currency { get; set; } = "IDR";
    public string TimeZone { get; set; } = "Asia/Makassar";
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Remarks { get; set; }
}
