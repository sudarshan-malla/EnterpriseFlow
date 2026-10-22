using EnterpriseFlow.Data;
using EnterpriseFlow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EnterpriseFlow.Controllers;
[ApiController, Route("api/[controller]")]
public class AuditController(AppDbContext db) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IEnumerable<AuditLog>>> Get([FromQuery]int limit=100) => await db.AuditLogs.OrderByDescending(x=>x.CreatedAtUtc).Take(Math.Clamp(limit,1,500)).ToListAsync();
}
