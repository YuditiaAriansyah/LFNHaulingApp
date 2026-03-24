using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models;

// ============ EMPLOYEE ============
[Table("hr_employees")]
public class Employee
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? NickName { get; set; }

    [MaxLength(50)]
    public string? PIN { get; set; }

    [MaxLength(50)]
    public string? RFID { get; set; }

    [MaxLength(10)]
    public string? Gender { get; set; } // MALE, FEMALE

    public DateTime? BirthDate { get; set; }

    [MaxLength(100)]
    public string? BirthPlace { get; set; }

    [MaxLength(50)]
    public string? Religion { get; set; }

    [MaxLength(50)]
    public string? MaritalStatus { get; set; } // SINGLE, MARRIED, DIVORCED

    [MaxLength(255)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(100)]
    public string? EmergencyContact { get; set; }

    [MaxLength(100)]
    public string? EmergencyPhone { get; set; }

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Position { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Level { get; set; } // STAFF, SUPERVISOR, MANAGER, DIRECTOR

    [MaxLength(50)]
    public string? EmployeeType { get; set; } // PERMANENT, CONTRACT, DAILY, INTERN

    [MaxLength(50)]
    public string? Status { get; set; } = "ACTIVE"; // ACTIVE, INACTIVE, RESIGNED, TERMINATED

    public DateTime? JoinDate { get; set; }

    public DateTime? ResignDate { get; set; }

    [MaxLength(255)]
    public string? PhotoUrl { get; set; }

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============ ATTENDANCE ============
[Table("hr_attendance")]
public class Attendance
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string EmployeeName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    public DateTime AttendanceDate { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    [MaxLength(50)]
    public string? Shift { get; set; } // SHIFT_1, SHIFT_2, SHIFT_3, OFF

    [MaxLength(50)]
    public string? Status { get; set; } = "PRESENT"; // PRESENT, ABSENT, LATE, SICK, LEAVE, PERMIT

    [Column(TypeName = "decimal(18,2)")]
    public decimal? WorkingHours { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? OvertimeHours { get; set; }

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============ PAYROLL ============
[Table("hr_payroll")]
public class Payroll
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string PayrollNumber { get; set; } = string.Empty;

    public DateTime PayrollDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(10)]
    public string PeriodMonth { get; set; } = string.Empty; // 01, 02, ... 12

    [Required]
    [MaxLength(4)]
    public string PeriodYear { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string EmployeeName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Site { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Position { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal BasicSalary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Allowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Overtime { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Bonus { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalEarning { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AbsenceDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal InsuranceDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OtherDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NetSalary { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "DRAFT"; // DRAFT, CALCULATED, APPROVED, PAID

    [MaxLength(255)]
    public string? Remarks { get; set; }

    public DateTime? PaidDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============ INPUT MODELS ============
public class EmployeeInput
{
    public string? EmployeeCode { get; set; }
    public string FullName { get; set; } = "";
    public string? NickName { get; set; }
    public string? PIN { get; set; }
    public string? RFID { get; set; }
    public string? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? BirthPlace { get; set; }
    public string? Religion { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public string Site { get; set; } = "";
    public string Department { get; set; } = "";
    public string Position { get; set; } = "";
    public string? Level { get; set; }
    public string? EmployeeType { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime? JoinDate { get; set; }
    public DateTime? ResignDate { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Remarks { get; set; }
}

public class AttendanceInput
{
    public string? EmployeeCode { get; set; }
    public string? EmployeeName { get; set; }
    public string Site { get; set; } = "";
    public string Department { get; set; } = "";
    public DateTime? AttendanceDate { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public string? Shift { get; set; }
    public string Status { get; set; } = "PRESENT";
    public decimal? WorkingHours { get; set; }
    public decimal? OvertimeHours { get; set; }
    public string? Remarks { get; set; }
}

public class PayrollInput
{
    public string? PayrollNumber { get; set; }
    public DateTime? PayrollDate { get; set; }
    public string PeriodMonth { get; set; } = "";
    public string PeriodYear { get; set; } = "";
    public string EmployeeCode { get; set; } = "";
    public string? EmployeeName { get; set; }
    public string Site { get; set; } = "";
    public string Department { get; set; } = "";
    public string Position { get; set; } = "";
    public decimal BasicSalary { get; set; }
    public decimal Allowance { get; set; }
    public decimal Overtime { get; set; }
    public decimal Bonus { get; set; }
    public decimal AbsenceDeduction { get; set; }
    public decimal TaxDeduction { get; set; }
    public decimal InsuranceDeduction { get; set; }
    public decimal OtherDeduction { get; set; }
    public string Status { get; set; } = "DRAFT";
    public string? Remarks { get; set; }
    public DateTime? PaidDate { get; set; }
}

// ============ SUMMARY MODELS ============
public class HRSummary
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int InactiveEmployees { get; set; }
    public int TotalAttendance { get; set; }
    public int PresentToday { get; set; }
    public int AbsentToday { get; set; }
    public int LateToday { get; set; }
    public decimal TotalPayroll { get; set; }
    public int PendingPayroll { get; set; }
}
