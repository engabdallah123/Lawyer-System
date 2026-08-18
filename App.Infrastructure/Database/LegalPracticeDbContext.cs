using System.Reflection;
using System.Text.Json;
using App.Domain.Audit.Entities;
using App.Domain.Cases.Entities;
using App.Domain.Clients.Entities;
using App.Domain.Common;
using App.Domain.Consultations.Entities;
using App.Domain.Documents.Entities;
using App.Domain.Finance.Entities;
using App.Domain.Hearings.Entities;
using App.Domain.Lookups;
using App.Domain.Notifications.Entities;
using App.Domain.Tasks.Entities;
using App.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace App.Infrastructure.Database;

/// <summary>
/// DbContext الرئيسي لنظام إدارة مكاتب المحاماة
/// يطبق Global Query Filter لـ ISoftDeletable
/// ويطبق Audit تلقائيًا عبر SaveChangesAsync
/// </summary>
public class LegalPracticeDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public LegalPracticeDbContext(DbContextOptions<LegalPracticeDbContext> options)
        : base(options)
    {
    }

    #region DbSets

    // Clients
    public DbSet<Client> Clients => Set<Client>();

    // Cases
    public DbSet<Case> Cases => Set<Case>();
    public DbSet<CaseParty> CaseParties => Set<CaseParty>();
    public DbSet<CaseAssignment> CaseAssignments => Set<CaseAssignment>();
    public DbSet<CaseTimeline> CaseTimelines => Set<CaseTimeline>();

    // Hearings
    public DbSet<Hearing> Hearings => Set<Hearing>();

    // Power of Attorney
    public DbSet<App.Domain.PowerOfAttorney.Entities.PowerOfAttorney> PowerOfAttorneys => Set<App.Domain.PowerOfAttorney.Entities.PowerOfAttorney>();

    // Consultations
    public DbSet<Consultation> Consultations => Set<Consultation>();

    // Finance
    public DbSet<FeeAgreement> FeeAgreements => Set<FeeAgreement>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    // Documents
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();

    // Tasks
    public DbSet<LegalTask> Tasks => Set<LegalTask>();

    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ReminderSetting> ReminderSettings => Set<ReminderSetting>();

    // Lookups
    public DbSet<CaseType> CaseTypes => Set<CaseType>();
    public DbSet<CaseStatus> CaseStatuses => Set<CaseStatus>();
    public DbSet<Court> Courts => Set<Court>();
    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();

    // Audit
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // تحميل جميع الـ Configurations من هذا الـ Assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // تطبيق Global Query Filter لـ Soft Delete
        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    /// <summary>
    /// تطبيق Global Query Filter لإخفاء السجلات المحذوفة منطقيًا
    /// </summary>
    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(LegalPracticeDbContext)
                    .GetMethod(nameof(ApplySoftDeleteFilter),
                        BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, [modelBuilder]);
            }
        }
    }

    private static void ApplySoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ISoftDeletable
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }

    /// <summary>
    /// Override SaveChangesAsync لتطبيق Audit تلقائيًا
    /// وملء حقول IAuditable
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var auditEntries = OnBeforeSaveChanges();

        var result = await base.SaveChangesAsync(cancellationToken);

        if (auditEntries.Count > 0)
        {
            foreach (var auditEntry in auditEntries)
            {
                AuditLogs.Add(auditEntry);
            }
            await base.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    /// <summary>
    /// تسجيل التغييرات قبل الحفظ لإنشاء AuditLog
    /// </summary>
    private List<AuditLog> OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditLog>();

        foreach (var entry in ChangeTracker.Entries())
        {
            // لا نسجل AuditLog لنفسه
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var entityName = entry.Entity.GetType().Name;
            var entityId = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id")?.CurrentValue?.ToString();

            string action = entry.State switch
            {
                EntityState.Added => "Created",
                EntityState.Modified => "Updated",
                EntityState.Deleted => "Deleted",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(action))
                continue;

            string? oldValues = null;
            string? newValues = null;

            if (entry.State == EntityState.Modified)
            {
                var modifiedProperties = entry.Properties
                    .Where(p => p.IsModified)
                    .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue?.ToString());

                var newProperties = entry.Properties
                    .Where(p => p.IsModified)
                    .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue?.ToString());

                oldValues = JsonSerializer.Serialize(modifiedProperties);
                newValues = JsonSerializer.Serialize(newProperties);
            }
            else if (entry.State == EntityState.Added)
            {
                var props = entry.Properties
                    .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue?.ToString());
                newValues = JsonSerializer.Serialize(props);
            }
            else if (entry.State == EntityState.Deleted)
            {
                var props = entry.Properties
                    .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue?.ToString());
                oldValues = JsonSerializer.Serialize(props);
            }

            var auditLog = new AuditLog(
                userId: null, // سيتم ملؤه من الـ Service layer
                action: action,
                entityName: entityName,
                entityId: entityId,
                oldValues: oldValues,
                newValues: newValues,
                ipAddress: null,
                userAgent: null);

            auditEntries.Add(auditLog);
        }

        return auditEntries;
    }
}
