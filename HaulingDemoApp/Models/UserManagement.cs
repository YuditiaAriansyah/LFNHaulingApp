using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

[Table("users")]
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string Role { get; set; } = "USER"; // ADMIN, MANAGER, USER, VIEWER, OPERATOR

    [MaxLength(20)]
    public string? SiteCode { get; set; }

    [MaxLength(50)]
    public string? Department { get; set; }

    [MaxLength(50)]
    public string? Position { get; set; }

    [MaxLength(50)]
    public string? EmployeeCode { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(50)]
    public string? LastLoginIP { get; set; }

    public DateTime? LastLoginAt { get; set; }

    [MaxLength(50)]
    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

[Table("user_sessions")]
public class UserSession
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string SessionToken { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
}

[Table("audit_logs")]
public class AuditLog
{
    [Key]
    public int Id { get; set; }

    [MaxLength(50)]
    public string? Username { get; set; }

    [MaxLength(50)]
    public string Action { get; set; } = string.Empty; // CREATE, UPDATE, DELETE, LOGIN, LOGOUT

    [MaxLength(50)]
    public string? Module { get; set; } // FLEET, FUEL, PR, PO, GR, GI, FINANCE, MASTER

    [MaxLength(50)]
    public string? RecordId { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? IPAddress { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class UserInput
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string FullName { get; set; } = "";
    public string? Email { get; set; }
    public string Role { get; set; } = "USER";
    public string? SiteCode { get; set; }
    public string? Department { get; set; }
    public string? Position { get; set; }
    public string? EmployeeCode { get; set; }
    public bool IsActive { get; set; } = true;
}

public class LoginInput
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? Role { get; set; }
    public string? SiteCode { get; set; }
    public string? Message { get; set; }
}
