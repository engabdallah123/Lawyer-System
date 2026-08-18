using App.Domain.Audit.Entities;
using App.Domain.Cases.Entities;
using App.Domain.Clients.Entities;
using App.Domain.Consultations.Entities;
using App.Domain.Documents.Entities;
using App.Domain.Finance.Entities;
using App.Domain.Hearings.Entities;
using App.Domain.Lookups;
using App.Domain.Notifications.Entities;
using App.Domain.PowerOfAttorney.Entities;
using App.Domain.Tasks.Entities;
using Shared.Domain;
using Shared.Domain.Abstractions;

namespace App.Domain
{
    public interface IAppUnitOfWork : IUnitOfWork
    {
        // Clients
        IBaseRepository<Client> Clients { get; }

        // Cases
        IBaseRepository<Case> Cases { get; }
        IBaseRepository<CaseParty> CaseParties { get; }
        IBaseRepository<CaseAssignment> CaseAssignments { get; }
        IBaseRepository<CaseTimeline> CaseTimelines { get; }

        // Hearings
        IBaseRepository<Hearing> Hearings { get; }

        // Power of Attorney
        IBaseRepository<PowerOfAttorney.Entities.PowerOfAttorney> PowerOfAttorneys { get; }

        // Consultations
        IBaseRepository<Consultation> Consultations { get; }

        // Finance
        IBaseRepository<FeeAgreement> FeeAgreements { get; }
        IBaseRepository<Payment> Payments { get; }
        IBaseRepository<Expense> Expenses { get; }
        IBaseRepository<Invoice> Invoices { get; }
        IBaseRepository<InvoiceItem> InvoiceItems { get; }

        // Documents
        IBaseRepository<Document> Documents { get; }
        IBaseRepository<DocumentVersion> DocumentVersions { get; }

        // Tasks
        IBaseRepository<LegalTask> Tasks { get; }

        // Notifications
        IBaseRepository<Notification> Notifications { get; }
        IBaseRepository<ReminderSetting> ReminderSettings { get; }

        // Lookups
        IBaseRepository<CaseType> CaseTypes { get; }
        IBaseRepository<CaseStatus> CaseStatuses { get; }
        IBaseRepository<Court> Courts { get; }
        IBaseRepository<DocumentType> DocumentTypes { get; }

        // Audit
        IBaseRepository<AuditLog> AuditLogs { get; }
    }
}
