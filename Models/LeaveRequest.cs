using System.ComponentModel.DataAnnotations;
namespace EnterpriseFlow.Models;
public class LeaveRequest
{
    public int Id { get; set; }
    [Required] public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    [Required, MaxLength(30)] public string LeaveType { get; set; } = "Annual";
    [Required, MaxLength(500)] public string Reason { get; set; } = "";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int Days { get; set; }
    [MaxLength(20)] public string Status { get; set; } = "Pending";
    public string? ManagerComment { get; set; }
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAtUtc { get; set; }
}
