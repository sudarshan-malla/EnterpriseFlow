using EnterpriseFlow.Models;
using Microsoft.EntityFrameworkCore;
namespace EnterpriseFlow.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().HasIndex(e => e.Email).IsUnique();
        modelBuilder.Entity<LeaveRequest>().HasOne(l => l.Employee).WithMany().HasForeignKey(l => l.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LeaveRequest>().Property(l => l.StartDate).HasConversion(v => v.ToString("yyyy-MM-dd"), v => DateOnly.Parse(v));
        modelBuilder.Entity<LeaveRequest>().Property(l => l.EndDate).HasConversion(v => v.ToString("yyyy-MM-dd"), v => DateOnly.Parse(v));
    }
}
