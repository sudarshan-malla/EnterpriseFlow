namespace EnterpriseFlow.Models;
public class AuditLog
{
    public int Id { get; set; }
    public string Actor { get; set; } = "System";
    public string Action { get; set; } = "";
    public string EntityType { get; set; } = "";
    public int EntityId { get; set; }
    public string Details { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
