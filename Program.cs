using EnterpriseFlow.Data;
using EnterpriseFlow.Models;
using EnterpriseFlow.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=enterprise-flow.db"));
builder.Services.AddScoped<LeaveWorkflowService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Employees.Any())
    {
        db.Employees.AddRange(
            new Employee { FullName="Aarav Sharma", Email="aarav@enterprise.local", Department="Engineering", Role="Employee", AnnualLeaveBalance=18 },
            new Employee { FullName="Priya Nair", Email="priya@enterprise.local", Department="People Operations", Role="Manager", AnnualLeaveBalance=20 },
            new Employee { FullName="Rohan Mehta", Email="rohan@enterprise.local", Department="Finance", Role="Employee", AnnualLeaveBalance=15 },
            new Employee { FullName="Isha Patel", Email="isha@enterprise.local", Department="Engineering", Role="Admin", AnnualLeaveBalance=21 });
        db.SaveChanges();
    }
}
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseDefaultFiles(); app.UseStaticFiles();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status="healthy", application="EnterpriseFlow" }));
app.Run();
