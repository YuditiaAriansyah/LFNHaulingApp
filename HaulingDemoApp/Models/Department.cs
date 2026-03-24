using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("department_master")]
public class Department
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string DeptCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DeptName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? ParentCode { get; set; }  // links to DeptCode of parent dept (null = top-level)

    [MaxLength(10)]
    public string Level { get; set; } = "DEPT";  // DIVISION, DEPARTMENT, SECTION

    [MaxLength(50)]
    public string? CostCenter { get; set; }

    [MaxLength(20)]
    public string? SiteCode { get; set; }   // FK to site_master.SiteCode

    [MaxLength(100)]
    public string? HeadName { get; set; }

    [MaxLength(50)]
    public string? HeadTitle { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "ACTIVE";

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class DepartmentInput
{
    public string DeptCode { get; set; } = "";
    public string DeptName { get; set; } = "";
    public string? ParentCode { get; set; }
    public string Level { get; set; } = "DEPARTMENT";
    public string? CostCenter { get; set; }
    public string? SiteCode { get; set; }
    public string? HeadName { get; set; }
    public string? HeadTitle { get; set; }
    public string? Remarks { get; set; }
}
