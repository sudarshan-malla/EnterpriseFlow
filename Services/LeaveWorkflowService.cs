using EnterpriseFlow.Data;
using EnterpriseFlow.Models;
using Microsoft.EntityFrameworkCore;
namespace EnterpriseFlow.Services;
public class LeaveWorkflowService(AppDbContext db)
{
    public async Task<LeaveRequest> SubmitAsync(int employeeId, string leaveType, DateOnly start, DateOnly end, string reason)
    {
        var employee = await db.Employees.FindAsync(employeeId) ?? throw new KeyNotFoundException("Employee not found.");
        if (start < DateOnly.FromDateTime(DateTime.Today)) throw new ArgumentException("Start date cannot be in the past.");
        if (end < start) throw new ArgumentException("End date must be on or after start date.");
        var days = end.DayNumber - start.DayNumber + 1;
        if (days > employee.AnnualLeaveBalance) throw new ArgumentException("Requested days exceed the employee's leave balance.");
        var request = new LeaveRequest { EmployeeId=employeeId, LeaveType=leaveType.Trim(), StartDate=start, EndDate=end, Days=days, Reason=reason.Trim() };
        db.LeaveRequests.Add(request);
        db.AuditLogs.Add(new AuditLog { Actor=employee.FullName, Action="Submitted", EntityType="LeaveRequest", Details=$"Submitted {days} day(s) of {leaveType} leave." });
        await db.SaveChangesAsync();
        return request;
    }
    public async Task<LeaveRequest> ReviewAsync(int requestId, string decision, string reviewer, string? comment)
    {
        var request = await db.LeaveRequests.Include(x => x.Employee).FirstOrDefaultAsync(x => x.Id == requestId) ?? throw new KeyNotFoundException("Leave request not found.");
        if (request.Status != "Pending") throw new InvalidOperationException("Only pending requests can be reviewed.");
        if (decision is not ("Approved" or "Rejected")) throw new ArgumentException("Decision must be Approved or Rejected.");
        if (decision == "Approved")
        {
            if (request.Employee!.AnnualLeaveBalance < request.Days) throw new InvalidOperationException("Insufficient leave balance at approval time.");
            request.Employee.AnnualLeaveBalance -= request.Days;
        }
        request.Status = decision; request.ManagerComment = comment?.Trim(); request.ReviewedAtUtc = DateTime.UtcNow;
        db.AuditLogs.Add(new AuditLog { Actor=string.IsNullOrWhiteSpace(reviewer) ? "Manager" : reviewer, Action=decision, EntityType="LeaveRequest", EntityId=request.Id, Details=$"{decision} request #{request.Id}. {comment}" });
        await db.SaveChangesAsync();
        return request;
    }
}
