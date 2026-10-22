using EnterpriseFlow.Data;
using EnterpriseFlow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EnterpriseFlow.Controllers;
[ApiController, Route("api/[controller]")]
public class EmployeesController(AppDbContext db) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IEnumerable<Employee>>> Get() => await db.Employees.OrderBy(e=>e.FullName).ToListAsync();
    [HttpGet("{id:int}")] public async Task<ActionResult<Employee>> GetById(int id) => await db.Employees.FindAsync(id) is { } e ? e : NotFound();
    [HttpPost] public async Task<ActionResult<Employee>> Create(Employee employee)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (await db.Employees.AnyAsync(e=>e.Email==employee.Email)) return Conflict(new { message="An employee with this email already exists." });
        employee.Id=0; db.Employees.Add(employee); await db.SaveChangesAsync();
        db.AuditLogs.Add(new AuditLog { Actor="Admin", Action="Created", EntityType="Employee", EntityId=employee.Id, Details=$"Created employee {employee.FullName}." }); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id=employee.Id }, employee);
    }
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, Employee input)
    {
        var e=await db.Employees.FindAsync(id); if(e is null) return NotFound();
        if(!ModelState.IsValid) return ValidationProblem(ModelState);
        if(await db.Employees.AnyAsync(x=>x.Id!=id && x.Email==input.Email)) return Conflict(new { message="Email is already in use." });
        e.FullName=input.FullName; e.Email=input.Email; e.Department=input.Department; e.Role=input.Role; e.AnnualLeaveBalance=input.AnnualLeaveBalance;
        db.AuditLogs.Add(new AuditLog { Actor="Admin", Action="Updated", EntityType="Employee", EntityId=id, Details=$"Updated employee {e.FullName}." }); await db.SaveChangesAsync(); return NoContent();
    }
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id)
    {
        var e=await db.Employees.FindAsync(id); if(e is null) return NotFound();
        if(await db.LeaveRequests.AnyAsync(l=>l.EmployeeId==id)) return Conflict(new { message="Cannot delete an employee with leave history." });
        db.Employees.Remove(e); db.AuditLogs.Add(new AuditLog { Actor="Admin", Action="Deleted", EntityType="Employee", EntityId=id, Details=$"Deleted employee {e.FullName}." }); await db.SaveChangesAsync(); return NoContent();
    }
}
