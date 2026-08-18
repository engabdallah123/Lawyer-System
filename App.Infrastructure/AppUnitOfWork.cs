using App.Domain;
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
using App.Infrastructure.Database;
using Shared.Domain;
using Shared.Infrastructure.Database;

namespace App.Infrastructure
{
    public class AppUnitOfWork : IAppUnitOfWork
    {
        private readonly LegalPracticeDbContext _context;

        public IBaseRepository<Client> Clients { get; private set; }
        public IBaseRepository<Case> Cases { get; private set; }
        public IBaseRepository<CaseParty> CaseParties { get; private set; }
        public IBaseRepository<CaseAssignment> CaseAssignments { get; private set; }
        public IBaseRepository<CaseTimeline> CaseTimelines { get; private set; }
        public IBaseRepository<Hearing> Hearings { get; private set; }
        public IBaseRepository<PowerOfAttorney> PowerOfAttorneys { get; private set; }
        public IBaseRepository<Consultation> Consultations { get; private set; }
        public IBaseRepository<FeeAgreement> FeeAgreements { get; private set; }
        public IBaseRepository<Payment> Payments { get; private set; }
        public IBaseRepository<Expense> Expenses { get; private set; }
        public IBaseRepository<Invoice> Invoices { get; private set; }
        public IBaseRepository<InvoiceItem> InvoiceItems { get; private set; }
        public IBaseRepository<Document> Documents { get; private set; }
        public IBaseRepository<DocumentVersion> DocumentVersions { get; private set; }
        public IBaseRepository<LegalTask> Tasks { get; private set; }
        public IBaseRepository<Notification> Notifications { get; private set; }
        public IBaseRepository<ReminderSetting> ReminderSettings { get; private set; }
        public IBaseRepository<CaseType> CaseTypes { get; private set; }
        public IBaseRepository<CaseStatus> CaseStatuses { get; private set; }
        public IBaseRepository<Court> Courts { get; private set; }
        public IBaseRepository<DocumentType> DocumentTypes { get; private set; }
        public IBaseRepository<AuditLog> AuditLogs { get; private set; }

        public AppUnitOfWork(LegalPracticeDbContext context)
        {
            _context = context;
            Clients = new BaseRepository<Client>(_context);
            Cases = new BaseRepository<Case>(_context);
            CaseParties = new BaseRepository<CaseParty>(_context);
            CaseAssignments = new BaseRepository<CaseAssignment>(_context);
            CaseTimelines = new BaseRepository<CaseTimeline>(_context);
            Hearings = new BaseRepository<Hearing>(_context);
            PowerOfAttorneys = new BaseRepository<PowerOfAttorney>(_context);
            Consultations = new BaseRepository<Consultation>(_context);
            FeeAgreements = new BaseRepository<FeeAgreement>(_context);
            Payments = new BaseRepository<Payment>(_context);
            Expenses = new BaseRepository<Expense>(_context);
            Invoices = new BaseRepository<Invoice>(_context);
            InvoiceItems = new BaseRepository<InvoiceItem>(_context);
            Documents = new BaseRepository<Document>(_context);
            DocumentVersions = new BaseRepository<DocumentVersion>(_context);
            Tasks = new BaseRepository<LegalTask>(_context);
            Notifications = new BaseRepository<Notification>(_context);
            ReminderSettings = new BaseRepository<ReminderSetting>(_context);
            CaseTypes = new BaseRepository<CaseType>(_context);
            CaseStatuses = new BaseRepository<CaseStatus>(_context);
            Courts = new BaseRepository<Court>(_context);
            DocumentTypes = new BaseRepository<DocumentType>(_context);
            AuditLogs = new BaseRepository<AuditLog>(_context);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
