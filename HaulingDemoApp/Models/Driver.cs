using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("driver_master")]
public class Driver
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string DriverCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? NIK { get; set; }   // KTP number

    [MaxLength(20)]
    public string? SIM { get; set; }   // Surat Izin Mengemudi

    [MaxLength(20)]
    public string SIMType { get; set; } = "B2";  // B1, B2, C

    [MaxLength(20)]
    public DateTime? DateOfBirth { get; set; }

    [MaxLength(10)]
    public string? Gender { get; set; }  // LAKI-LAKI, PEREMPUAN

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Address { get; set; }

    [MaxLength(30)]
    public string? SiteCode { get; set; }

    [MaxLength(30)]
    public string? DepartmentCode { get; set; }

    [MaxLength(20)]
    public DateTime? JoinDate { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "ACTIVE";  // ACTIVE, OFF_DUTY, TERMINATED

    [MaxLength(50)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class DriverInput
{
    public string DriverCode { get; set; } = "";
    public string FullName { get; set; } = "";
    public string? NIK { get; set; }
    public string? SIM { get; set; }
    public string SIMType { get; set; } = "B2";
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? SiteCode { get; set; }
    public string? DepartmentCode { get; set; }
    public DateTime? JoinDate { get; set; }
    public string? Remarks { get; set; }
}
