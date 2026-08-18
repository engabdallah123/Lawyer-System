using Shared.Domain;

namespace App.Domain.Audit.Entities;

/// <summary>
/// سجل التدقيق — يسجل كل عملية حساسة في النظام
/// </summary>
public sealed class AuditLog : Entity
{
    public string? UserId { get; private set; }
    public string Action { get; private set; } = null!;
    public string EntityName { get; private set; } = null!;
    public string? EntityId { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTime Timestamp { get; private set; }

    private AuditLog() { }

    public AuditLog(
        string? userId,
        string action,
        string entityName,
        string? entityId,
        string? oldValues,
        string? newValues,
        string? ipAddress,
        string? userAgent)
        : base(Guid.NewGuid())
    {
        UserId = userId;
        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        OldValues = oldValues;
        NewValues = newValues;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Timestamp = DateTime.UtcNow;
    }
}
