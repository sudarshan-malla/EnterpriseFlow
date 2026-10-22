using EnterpriseFlow.Data;
using EnterpriseFlow.Models;
using EnterpriseFlow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EnterpriseFlow.Controllers;
[ApiController, Route("api/[controller]")]
public class LeaveRequestsController(AppDbContext db, LeaveWorkflowService workflow) : ControllerBase
{
    public record CreateLeaveRequest(int EmployeeId, string LeaveType, DateOnly StartDate, DateOnly EndDate, string Reason);
    public record ReviewLeaveRequest(string Decision, string Reviewer, string? Comment);
    [HttpGet] public async Task<ActionResult<IEnumerable<LeaveRequest>>> Get([FromQuery] string? status, [FromQuery] int? employeeId)
    {
        var query=db.LeaveRequests.Include(x=>x.Employee).AsQueryable();
        if(!string.IsNullOrWhiteSpace(status)) query=query.Where(x=>x.Status==status);
        if(employeeId.HasValue) query=query.Where(x=>x.EmployeeId==employeeId.Value);
        return await query.OrderByDescending(x=>x.SubmittedAtUtc).ToListAsync();
    }
    [HttpPost] public async Task<IActionResult> Create(CreateLeaveRequest input)
    {
        try { var result=await workflow.SubmitAsync(input.EmployeeId,input.LeaveType,input.StartDate,input.EndDate,input.Reason); return CreatedAtAction(nameof(Get),new { id=result.Id },result); }
        catch(KeyNotFoundException ex){return NotFound(new {message=ex.Message});}
        catch(ArgumentException ex){return BadRequest(new {message=ex.Message});}
    }
    [HttpPost("{id:int}/review")] public async Task<IActionResult> Review(int id, ReviewLeaveRequest input)
    {
        try { return Ok(await workflow.ReviewAsync(id,input.Decision,input.Reviewer,input.Comment)); }
        catch(KeyNotFoundException ex){return NotFound(new {message=ex.Message});}
        catch(ArgumentException ex){return BadRequest(new {message=ex.Message});}
        catch(InvalidOperationException ex){return Conflict(new {message=ex.Message});}
    }
    [HttpGet("summary")] public async Task<IActionResult> Summary()
    {
        var requests=await db.LeaveRequests.ToListAsync();
        return Ok(new { totalEmployees=await db.Employees.CountAsync(), totalRequests=requests.Count, pending=requests.Count(x=>x.Status=="Pending"), approved=requests.Count(x=>x.Status=="Approved"), rejected=requests.Count(x=>x.Status=="Rejected") });
    }
}
