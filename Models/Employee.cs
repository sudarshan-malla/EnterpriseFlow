using System.ComponentModel.DataAnnotations;
namespace EnterpriseFlow.Models;
public class Employee
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string FullName { get; set; } = "";
    [Required, EmailAddress, MaxLength(150)] public string Email { get; set; } = "";
    [Required, MaxLength(80)] public string Department { get; set; } = "";
    [Required, MaxLength(30)] public string Role { get; set; } = "Employee";
    [Range(0, 100)] public int AnnualLeaveBalance { get; set; } = 18;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
