# Legal Practice Management System — Agent Guidelines

> هذا الملف هو المرجع الأساسي لوكلاء الذكاء الاصطناعي والمطورين عند بناء وتوسعة نظام إدارة مكاتب المحاماة.
>
> النظام **ليس Modular Monolith متعدد الوحدات البرمجية**؛ هو نظام واحد (Single Business Module) منظم داخليًا إلى مجالات عمل/مجلدات واضحة، مع طبقات Shared مشتركة.

---

## 1. Solution Overview

النظام هو **Legal Practice Management System** احترافي لإدارة مكاتب المحاماة، باللغة العربية بالكامل ويدعم RTL.

### التقنيات

| التقنية | الاستخدام |
|---|---|
| ASP.NET Core 10 | المنصة الأساسية |
| Blazor Web App | Presentation |
| Interactive Server | التفاعل داخل Blazor |
| Entity Framework Core | ORM وعمليات الكتابة |
| SQL Server | قاعدة البيانات |
| ASP.NET Core Identity | المستخدمون والمصادقة |
| Permission-Based Authorization | الصلاحيات الدقيقة |
| MudBlazor | UI + RTL |
| SignalR | الإشعارات والتحديثات الفورية |
| QuestPDF | التقارير والفواتير والمستندات PDF |
| Excel Export | تصدير التقارير |
| FluentValidation | التحقق من المدخلات |
| MediatR | CQRS / Application pipeline |
| Serilog | Logging |
| Soft Delete | الحذف المنطقي |
| Audit Log | تسجيل كامل للتغييرات |

### بنية المشاريع

النظام يتكون من **مشروع Business Module واحد فقط**، وليس مشروعًا منفصلًا لكل وحدة أعمال.

```text
Lawyer_System.sln
│
├── Shared.Domain
├── Shared.Application
├── Shared.Infrastructure
│
├── App.Domain
├── App.Application
├── App.Infrastructure
│
└── App.Web
```

### مسؤولية كل مشروع

| المشروع | المسؤولية |
|---|---|
| `Shared.Domain` | أساسيات Domain المشتركة |
| `Shared.Application` | CQRS abstractions + behaviors + services contracts |
| `Shared.Infrastructure` | خدمات البنية التحتية المشتركة |
| `App.Domain` | جميع الـ Entities وقواعد العمل الخاصة بالنظام |
| `App.Application` | Commands / Queries / DTOs / Validators / Use Cases |
| `App.Infrastructure` | EF Core + SQL Server + Identity + Repositories + Audit + Files |
| `App.Web` | Blazor Web App + MudBlazor + Authentication + SignalR UI |

---

# 2. Architecture

## Clean Architecture

```text
Shared Domain
      ↑
Legal Domain
      ↑
Legal Application
      ↑
Legal Infrastructure
      ↑
Legal Web
```

والقاعدة الأساسية:

```text
Domain
  ↓
Application
  ↓
Infrastructure
  ↓
Presentation
```

### Dependency Rules

- `App.Domain` لا يعتمد على Infrastructure أو Web.
- `App.Application` يعتمد على Domain وShared Application.
- `App.Infrastructure` يعتمد على Application + Domain + Shared Infrastructure.
- `App.Web` يعتمد على Infrastructure + Application.
- لا يتم إنشاء `Clients.Domain` أو `Cases.Domain` أو `Finance.Domain` كمشاريع منفصلة.
- كل مجالات النظام موجودة داخل `App.Domain/Application/Infrastructure`.

---

# 3. Internal Business Areas

رغم أن النظام مشروع Business Module واحد، يتم تنظيم الكود داخله إلى مجالات واضحة:

```text
App.Domain/
├── Clients/
├── Cases/
├── Hearings/
├── PowerOfAttorney/
├── Consultations/
├── Finance/
├── Documents/
├── Tasks/
├── Notifications/
├── Users/
├── Audit/
└── Shared/
```

هذه **مجلدات تنظيمية فقط** وليست Modules مستقلة.

مثال:

```text
App.Domain/
└── Cases/
    ├── Entities/
    │   ├── Case.cs
    │   ├── CaseParty.cs
    │   ├── CaseAssignment.cs
    │   └── CaseTimeline.cs
    ├── Enums/
    ├── Errors/
    └── Interfaces/
```

---

# 4. Domain Layer

## 4.1 Entity Convention

جميع الـ Entities المهمة ترث من:

```csharp
Entity
```

وتستخدم:

- `Guid` كمفتاح أساسي.
- `private set`.
- Constructor خاص لـ EF Core.
- Factory method عند وجود قواعد إنشاء.
- Domain methods لتغيير الحالة.
- Domain Events عند الحاجة.
- Soft Delete حيث يكون الحذف المنطقي مطلوبًا.

مثال:

```csharp
public sealed class Client : Entity
{
    public string? FullName { get; private set; }
    public string? CompanyName { get; private set; }

    private Client() { }

    private Client(Guid id, ...)
        : base(id)
    {
        ...
    }

    public static Result<Client> Create(...)
    {
        ...
    }

    public Result Update(...)
    {
        ...
    }
}
```

---

# 5. Core Entities

## 5.1 Client — الموكل

```text
Client
- Id : Guid
- ClientType : ClientType
- FullName : string?
- CompanyName : string?
- NationalId : string?
- CommercialRegister : string?
- Phone : string
- Mobile : string?
- Email : string?
- Address : string?
- City : string?
- Notes : string?
- IsActive : bool
- IsDeleted : bool
- CreatedAt : DateTime
- CreatedBy : string
- UpdatedAt : DateTime?
- UpdatedBy : string?
```

### Business Rules

- الشخص الطبيعي يحتاج `FullName`.
- الشركة تحتاج `CompanyName`.
- رقم الهاتف الأساسي مطلوب.
- لا يسمح بتكرار رقم الهوية إذا تم استخدامه.
- لا يسمح بتكرار السجل التجاري للشركات.
- حذف الموكل يكون Soft Delete.
- لا يسمح بحذف موكل لديه بيانات قانونية مرتبطة دون إجراء أرشفة مناسب.

---

## 5.2 Case — القضية

```text
Case
- Id : Guid
- InternalNumber : string
- CourtNumber : string?
- Title : string
- CaseTypeId : int
- CaseStatusId : int
- CourtId : int?
- Circuit : string?
- JudgeName : string?
- OpenDate : DateTime
- CloseDate : DateTime?
- ClaimAmount : decimal?
- Description : string?
- CurrentStage : string?
- Notes : string?
- IsDeleted : bool
- CreatedAt : DateTime
- CreatedBy : string
- UpdatedAt : DateTime?
- UpdatedBy : string?
```

### علاقات القضية

```text
Case
 ├── Client / CaseParty
 ├── CaseAssignment
 ├── Hearings
 ├── CaseTimeline
 ├── Documents
 ├── Tasks
 ├── FeeAgreement
 ├── Payments
 ├── Expenses
 └── PowerOfAttorney
```

---

## 5.3 CaseParty — أطراف القضية

```text
CaseParty
- Id : Guid
- CaseId : Guid
- ClientId : Guid?
- PartyName : string?
- PartyRole : PartyRole
- IsMainClient : bool
- Notes : string?
```

`PartyRole`:

```csharp
Plaintiff
Defendant
Witness
OtherLawyer
Other
```

يمكن أن يكون الطرف موكلًا موجودًا في النظام أو اسمًا حرًا عند عدم وجود Client.

---

## 5.4 CaseAssignment — توزيع القضية

```text
CaseAssignment
- Id : Guid
- CaseId : Guid
- UserId : string
- RoleInCase : string
- AssignedDate : DateTime
- Notes : string?
```

أدوار مقترحة:

```text
محامي أساسي
محامي مساعد
متدرب
```

يجب تسجيل أي تعيين أو إزالة تعيين في Audit Log وTimeline عند الحاجة.

---

## 5.5 Hearing — الجلسات

```text
Hearing
- Id : Guid
- CaseId : Guid
- HearingDate : DateTime
- HearingTime : TimeSpan?
- HearingType : string
- Result : string?
- Notes : string?
- NextHearingDate : DateTime?
- CreatedAt : DateTime
- CreatedBy : string
```

### قواعد

- الجلسة مرتبطة دائمًا بقضية.
- لا يتم اعتبار الجلسة مكتملة إلا بعد تسجيل نتيجتها أو حالتها.
- عند تحديد جلسة قادمة يجب إنشاء/تحديث التنبيهات.
- عند تعديل موعد جلسة يجب إعادة جدولة التنبيهات.

---

## 5.6 CaseTimeline

```text
CaseTimeline
- Id : Guid
- CaseId : Guid
- Title : string
- Description : string?
- IsImportant : bool
- CreatedAt : DateTime
- CreatedBy : string
```

الـ Timeline هو السجل الزمني التشغيلي للقضية.

أمثلة:

```text
تم فتح القضية
تم تعيين المحامي أحمد
تم رفع صحيفة الدعوى
تم تحديد جلسة
تم حضور الجلسة
تم تسجيل نتيجة الجلسة
تم رفع مستند جديد
تم تسجيل دفعة
تم تغيير حالة القضية
```

---

# 6. PowerOfAttorney — التوكيلات

```text
PowerOfAttorney
- Id : Guid
- ClientId : Guid
- CaseId : Guid?
- PowerNumber : string
- IssueDate : DateTime
- ExpiryDate : DateTime?
- NotaryName : string?
- NotaryNumber : string?
- FilePath : string?
- Notes : string?
- IsActive : bool
- CreatedAt : DateTime
- CreatedBy : string
```

### قواعد

- التوكيل مرتبط بموكل.
- يمكن ربطه بقضية أو تركه عامًا للموكل.
- يجب تنبيه المستخدم قبل انتهاء التوكيل.
- رفع الملف يتم عبر `IFileService`.
- لا يتم تخزين الملفات الكبيرة داخل SQL Server إلا إذا كان هناك سبب واضح؛ يتم تخزين metadata في DB والملف في storage مناسب.

---

# 7. Consultation — الاستشارات

```text
Consultation
- Id : Guid
- ClientId : Guid
- ConsultationDate : DateTime
- Subject : string
- Description : string?
- Fee : decimal?
- Status : ConsultationStatus
- Notes : string?
- CreatedAt : DateTime
- CreatedBy : string
```

`ConsultationStatus`:

```text
Scheduled
Completed
Cancelled
```

الاستشارة لا يجب أن تكون شرطًا لفتح قضية.

يمكن أن يكون المسار:

```text
Client
  ↓
Consultation
  ↓
Case
```

أو:

```text
Client
  ↓
Case مباشرة
```

---

# 8. Finance

## 8.1 FeeAgreement

```text
FeeAgreement
- Id : Guid
- CaseId : Guid?
- ClientId : Guid
- AgreementType : AgreementType
- TotalAmount : decimal
- PaidAmount : decimal
- Description : string?
- StartDate : DateTime
- EndDate : DateTime?
- CreatedAt : DateTime
- CreatedBy : string
```

`AgreementType`:

```text
Fixed
Percentage
Hourly
```

---

## 8.2 Payment

```text
Payment
- Id : Guid
- ClientId : Guid
- CaseId : Guid?
- FeeAgreementId : Guid?
- InvoiceId : Guid?
- Amount : decimal
- PaymentDate : DateTime
- PaymentMethod : string
- ReferenceNumber : string?
- Notes : string?
- ReceivedBy : string
- CreatedAt : DateTime
```

---

## 8.3 Expense

```text
Expense
- Id : Guid
- CaseId : Guid?
- ExpenseType : string
- Amount : decimal
- ExpenseDate : DateTime
- Description : string?
- ReceiptPath : string?
- PaidBy : string
- CreatedAt : DateTime
- CreatedBy : string
```

---

## 8.4 Invoice

يجب إضافة كيان Invoice لأن النظام يحتاج فواتير رسمية.

```text
Invoice
- Id : Guid
- InvoiceNumber : string
- ClientId : Guid
- CaseId : Guid?
- FeeAgreementId : Guid?
- IssueDate : DateTime
- DueDate : DateTime?
- SubTotal : decimal
- Discount : decimal
- Tax : decimal
- TotalAmount : decimal
- PaidAmount : decimal
- Status : InvoiceStatus
- Notes : string?
- CreatedAt : DateTime
- CreatedBy : string
```

حالات الفاتورة:

```text
Draft
Issued
PartiallyPaid
Paid
Cancelled
Overdue
```

---

# 9. Documents — إدارة المستندات

يجب أن يدعم النظام Versioning.

## Document

```text
Document
- Id : Guid
- CaseId : Guid?
- ClientId : Guid?
- DocumentTypeId : int?
- Name : string
- Description : string?
- CurrentVersionId : Guid?
- IsDeleted : bool
- CreatedAt : DateTime
- CreatedBy : string
```

## DocumentVersion

```text
DocumentVersion
- Id : Guid
- DocumentId : Guid
- VersionNumber : int
- FilePath : string
- FileName : string
- ContentType : string
- FileSize : long
- UploadedAt : DateTime
- UploadedBy : string
- Notes : string?
```

### قواعد Versioning

عند رفع نسخة جديدة:

```text
Document
  ↓
DocumentVersion 1
  ↓
DocumentVersion 2
  ↓
DocumentVersion 3
```

لا يتم حذف النسخ القديمة.

---

# 10. Tasks — المهام

```text
LegalTask
- Id : Guid
- CaseId : Guid?
- AssignedToUserId : string
- Title : string
- Description : string?
- DueDate : DateTime?
- Priority : TaskPriority
- Status : TaskStatus
- CompletedAt : DateTime?
- CreatedAt : DateTime
- CreatedBy : string
```

### Task Status

```text
Pending
InProgress
Completed
Cancelled
```

### Task Priority

```text
Low
Normal
High
Urgent
```

---

# 11. Notifications

يجب دعم:

```text
In-App
Email
WhatsApp
```

لكن Notification Domain لا يرتبط مباشرة بمزود خارجي.

يتم استخدام abstraction:

```csharp
INotificationService
IEmailService
IWhatsAppService
```

ويتم تنفيذها في Infrastructure.

## Notification

```text
Notification
- Id : Guid
- UserId : string
- Title : string
- Message : string
- Type : NotificationType
- IsRead : bool
- CreatedAt : DateTime
- ReadAt : DateTime?
```

---

# 12. Identity & Permissions

يستخدم:

```text
ASP.NET Core Identity
```

ويتم إضافة Permission-based authorization فوق Roles.

## أمثلة Roles

```text
Owner
Partner
Lawyer
Secretary
Accountant
Administrator
```

لكن الـ Role وحده لا يكفي.

## Permissions

أمثلة:

```text
Clients.View
Clients.Create
Clients.Edit
Clients.Delete

Cases.View
Cases.Create
Cases.Edit
Cases.Delete
Cases.Assign

Hearings.View
Hearings.Create
Hearings.Edit
Hearings.Delete

Documents.View
Documents.Upload
Documents.Delete

Finance.View
Payments.Create
Invoices.Create
Expenses.Create

Reports.View
Reports.Export

Users.View
Users.Manage
Permissions.Manage

Audit.View
```

يتم تطبيق الصلاحيات على مستوى:

```text
Page
Component
Command
Query
Action
```

---

# 13. Audit Log

كل عملية حساسة يجب تسجيلها.

## AuditLog

```text
AuditLog
- Id : Guid
- UserId : string?
- Action : string
- EntityName : string
- EntityId : string?
- OldValues : string?
- NewValues : string?
- IpAddress : string?
- UserAgent : string?
- Timestamp : DateTime
```

### أمثلة

```text
Created Client
Updated Client
Deleted Client
Created Case
Assigned Lawyer
Created Hearing
Changed Hearing Date
Uploaded Document
Created Payment
Issued Invoice
Changed Permission
```

يفضل تنفيذ Audit على مستوى `DbContext SaveChanges` مع إمكانية إضافة Audit Events للعمليات التي لا تظهر كتغيير EF مباشر.

---

# 14. Soft Delete

يجب أن يدعم النظام Soft Delete.

الكيانات المناسبة تحتوي:

```text
IsDeleted
DeletedAt
DeletedBy
```

عند تنفيذ Delete:

```text
UPDATE ...
SET IsDeleted = 1
```

وليس:

```text
DELETE FROM ...
```

يتم استخدام EF Core Global Query Filters لإخفاء السجلات المحذوفة افتراضيًا.

يجب أن يكون هناك إمكانية للإدارة لاستعراض السجلات المحذوفة واستعادتها إذا كانت الصلاحية تسمح.

---

# 15. CQRS

كل Use Case مهم يتم تنفيذه من خلال:

```text
Command
CommandHandler
CommandValidator
```

أو:

```text
Query
QueryHandler
Response DTO
```

مثال:

```text
CreateClientCommand
CreateClientCommandHandler
CreateClientCommandValidator
```

```text
GetClientByIdQuery
GetClientByIdQueryHandler
ClientResponse
```

### Pipeline

```text
Request
 ↓
Authorization
 ↓
ValidationBehavior
 ↓
LoggingBehavior
 ↓
Handler
 ↓
Domain
 ↓
UnitOfWork
```

---

# 16. Project Structure

## Domain

```text
App.Domain/
├── Clients/
│   ├── Entities/
│   ├── Enums/
│   ├── Errors/
│   └── Interfaces/
│
├── Cases/
│   ├── Entities/
│   ├── Enums/
│   ├── Errors/
│   └── Interfaces/
│
├── Hearings/
├── PowerOfAttorney/
├── Consultations/
├── Finance/
├── Documents/
├── Tasks/
├── Notifications/
├── Audit/
│
├── Common/
│
└── App.Domain.csproj
```

## Application

```text
App.Application/
├── Clients/
│   ├── Commands/
│   └── Queries/
│
├── Cases/
│   ├── Commands/
│   └── Queries/
│
├── Hearings/
├── PowerOfAttorney/
├── Consultations/
├── Finance/
├── Documents/
├── Tasks/
├── Notifications/
├── Reports/
├── Search/
├── Dashboard/
│
├── Behaviors/
├── DependencyInjection.cs
└── App.Application.csproj
```

## Infrastructure

```text
App.Infrastructure/
├── Database/
│   ├── LegalPracticeDbContext.cs
│   └── Migrations/
│
├── Configurations/
│   ├── ClientConfiguration.cs
│   ├── CaseConfiguration.cs
│   ├── HearingConfiguration.cs
│   ├── InvoiceConfiguration.cs
│   └── ...
│
├── Repositories/
├── Identity/
├── Authorization/
├── Audit/
├── Files/
├── Notifications/
│   ├── Email/
│   └── WhatsApp/
├── Reports/
├── Search/
├── Services/
├── UnitOfWork/
├── DependencyInjection.cs
└── App.Infrastructure.csproj
```

## Web

```text
App.Web/
├── Components/
│   ├── Layout/
│   ├── Pages/
│   │   ├── Dashboard/
│   │   ├── Clients/
│   │   ├── Cases/
│   │   ├── Hearings/
│   │   ├── Consultations/
│   │   ├── Finance/
│   │   ├── Documents/
│   │   ├── Tasks/
│   │   ├── Reports/
│   │   └── Users/
│   └── Shared/
│
├── Hubs/
├── Services/
├── wwwroot/
└── Program.cs
```

---

# 17. Database

SQL Server + EF Core.

يجب استخدام:

```csharp
LegalPracticeDbContext
```

ويحتوي على DbSets لكل الكيانات.

مثال:

```csharp
public DbSet<Client> Clients => Set<Client>();
public DbSet<Case> Cases => Set<Case>();
public DbSet<CaseParty> CaseParties => Set<CaseParty>();
public DbSet<CaseAssignment> CaseAssignments => Set<CaseAssignment>();
public DbSet<Hearing> Hearings => Set<Hearing>();
public DbSet<CaseTimeline> CaseTimelines => Set<CaseTimeline>();
public DbSet<PowerOfAttorney> PowerOfAttorneys => Set<PowerOfAttorney>();
public DbSet<Consultation> Consultations => Set<Consultation>();
public DbSet<FeeAgreement> FeeAgreements => Set<FeeAgreement>();
public DbSet<Payment> Payments => Set<Payment>();
public DbSet<Expense> Expenses => Set<Expense>();
public DbSet<Invoice> Invoices => Set<Invoice>();
public DbSet<Document> Documents => Set<Document>();
public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
public DbSet<LegalTask> Tasks => Set<LegalTask>();
public DbSet<Notification> Notifications => Set<Notification>();
public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
```

---

# 18. EF Core Configuration

كل Entity له Configuration منفصل:

```text
Configurations/
├── ClientConfiguration.cs
├── CaseConfiguration.cs
├── CasePartyConfiguration.cs
├── CaseAssignmentConfiguration.cs
├── HearingConfiguration.cs
├── CaseTimelineConfiguration.cs
├── PowerOfAttorneyConfiguration.cs
├── ConsultationConfiguration.cs
├── FeeAgreementConfiguration.cs
├── PaymentConfiguration.cs
├── ExpenseConfiguration.cs
├── InvoiceConfiguration.cs
├── DocumentConfiguration.cs
├── DocumentVersionConfiguration.cs
├── LegalTaskConfiguration.cs
├── NotificationConfiguration.cs
└── AuditLogConfiguration.cs
```

يتم تحميلها بواسطة:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(
    Assembly.GetExecutingAssembly());
```

---

# 19. Repository

لا يتم إنشاء Repository مخصص إلا عند الحاجة.

القاعدة:

```text
IBaseRepository<T>
```

يكفي للعمليات العامة.

يتم إنشاء:

```text
IClientRepository
ICaseRepository
IHearingRepository
```

فقط عندما تكون هناك استعلامات متخصصة لا يناسبها الـ Base Repository.

---

# 20. Unit of Work

يوجد Unit of Work واحد للنظام:

```csharp
ILegalPracticeUnitOfWork
```

وليس Unit of Work لكل مجال.

مثال:

```csharp
public interface ILegalPracticeUnitOfWork
{
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
```

يمكن استخدام repositories متعددة ضمن نفس transaction.

---

# 21. Dashboard

## المحامي

عند الدخول يرى:

```text
جلسات اليوم
جلسات هذا الأسبوع
القضايا المسندة إليه
المهام المتأخرة
المهام القادمة
آخر تحديثات القضايا
التنبيهات
```

## صاحب المكتب / الشريك

يرى:

```text
إجمالي القضايا
القضايا المفتوحة
القضايا المغلقة
جلسات اليوم
جلسات الأسبوع
إجمالي المستحقات
المدفوعات
المتأخرات
المصروفات
صافي التحصيل
أداء المحامين
أكثر أنواع القضايا
القضايا القريبة من المواعيد المهمة
```

---

# 22. Global Search

البحث العام يبحث في:

```text
Clients
Cases
Hearings
PowerOfAttorney
Consultations
Documents
Invoices
Payments
Tasks
```

مثال:

```text
"أحمد محمد"
```

قد يعيد:

```text
موكل
قضية
توكيل
فاتورة
مستند
```

ويجب احترام صلاحيات المستخدم أثناء البحث.

---

# 23. التقارير

التقارير المطلوبة:

```text
تقرير القضايا
تقرير الجلسات
تقرير الموكلين
تقرير التحصيل
تقرير المتأخرات
تقرير المصروفات
تقرير الأتعاب
تقرير أداء المحامين
تقرير المستندات
تقرير المهام
Audit Report
```

التصدير:

```text
PDF → QuestPDF
Excel → Excel Export Service
```

---

# 24. Scenario 1 — استقبال موكل جديد

## المسار الأساسي

```text
استقبال الموكل
      ↓
البحث هل الموكل موجود؟
      ↓
إن كان غير موجود → إنشاء Client
      ↓
تحديد نوع العميل
      ↓
إضافة بيانات الاتصال
      ↓
اختيار:
   ├── استشارة
   └── قضية مباشرة
```

### هل الاستشارة إلزامية؟

لا.

يوجد مساران:

### مسار الاستشارة

```text
Client
 ↓
Consultation
 ↓
Completed
 ↓
قرار بفتح قضية
 ↓
Case
```

### مسار القضية المباشرة

```text
Client
 ↓
Case
```

### رفع التوكيل

بعد تسجيل الموكل:

```text
Client
 ↓
PowerOfAttorney
 ↓
Upload Document
```

يمكن ربط التوكيل بالقضية لاحقًا.

### عقد الأتعاب

بعد فتح القضية أو الاتفاق على الخدمة:

```text
Client
 ↓
FeeAgreement
 ↓
Total Amount
 ↓
Invoice
 ↓
Payment
```

---

# 25. Scenario 2 — فتح قضية جديدة

## الخطوات

```text
اختيار الموكل
 ↓
إنشاء Case
 ↓
تحديد نوع القضية
 ↓
تحديد المحكمة والدائرة
 ↓
إدخال رقم القضية
 ↓
إضافة الوصف
 ↓
إضافة الأطراف
 ↓
تعيين المحامي
 ↓
رفع المستندات
 ↓
إضافة Timeline Entry
```

### الأطراف

```text
Case
 ├── Plaintiff
 ├── Defendant
 ├── Witness
 ├── Other Lawyer
 └── Other
```

### توزيع القضية

```text
Case
 ↓
CaseAssignment
 ↓
Lawyer
 ↓
RoleInCase
```

### المستندات

```text
Case
 ↓
Document
 ↓
DocumentVersion
```

### Timeline

عند كل حدث مهم:

```text
CaseTimeline
```

ويظهر في صفحة القضية بترتيب زمني تنازلي.

---

# 26. Scenario 3 — إدارة الجلسات

## إضافة جلسة

من صفحة القضية:

```text
Case
 ↓
جلسات
 ↓
إضافة جلسة
 ↓
التاريخ
 ↓
الوقت
 ↓
نوع الجلسة
 ↓
حفظ
```

## التقويم الشهري

يعرض Calendar شهري باستخدام MudBlazor أو Component مخصص.

مثال:

```text
الأحد الإثنين الثلاثاء الأربعاء الخميس الجمعة السبت
                         1       2       3
  4       5       6       7       8       9      10
```

اليوم الذي يحتوي على جلسات يعرض:

```text
● 3 جلسات
```

وعند الضغط على اليوم:

```text
جلسات اليوم
├── 09:00 — قضية رقم 100
├── 11:00 — قضية رقم 250
└── 14:00 — قضية رقم 312
```

## التنبيهات

عند إنشاء جلسة:

```text
Hearing Created
      ↓
Notification Scheduler
      ↓
قبل الجلسة
├── In-App
├── Email
└── WhatsApp إذا كان مفعلًا
```

التنبيهات قابلة للتخصيص.

مثال:

```text
7 أيام قبل
24 ساعة قبل
2 ساعة قبل
```

ولا يتم إرسال WhatsApp إلا إذا كان التكامل مفعلاً والمستخدم/الموكل لديه موافقة وإعداد مناسب.

## بعد انتهاء الجلسة

المحامي يفتح الجلسة:

```text
تسجيل النتيجة
 ↓
إضافة ملاحظات
 ↓
تحديد جلسة قادمة
 ↓
حفظ
```

ويتم:

```text
CaseTimeline Entry
+
Notification Scheduling
+
AuditLog
```

---

# 27. Scenario 4 — التحصيل والمتابعة المالية

## تسجيل دفعة

```text
Client
 ↓
Payment
 ↓
اختيار:
   FeeAgreement
   Invoice
   Case
 ↓
Amount
 ↓
PaymentMethod
 ↓
Save
```

بعد الحفظ:

```text
Payment Created
 ↓
Update Invoice PaidAmount
 ↓
Update FeeAgreement PaidAmount
 ↓
Recalculate Outstanding
 ↓
AuditLog
```

## الفاتورة

```text
FeeAgreement
 ↓
Create Invoice
 ↓
Issue Invoice
 ↓
Generate PDF
```

## المستحقات

Dashboard المالي:

```text
إجمالي الأتعاب
- المدفوع
= المتبقي
```

مع تصنيف:

```text
مستحق
متأخر
مدفوع جزئيًا
مدفوع بالكامل
```

---

# 28. Scenario 5 — العمل اليومي للمحامي

عند الدخول:

```text
Dashboard
```

يعرض:

### اليوم

```text
جلساتي اليوم
مهامي المستحقة
المهام المتأخرة
التنبيهات
```

### الأسبوع

```text
جلسات الأسبوع
المهام القادمة
القضايا التي تحتاج تحديثًا
```

### القضايا

```text
القضايا المسندة
آخر تحديث
آخر جلسة
الجلسة القادمة
حالة القضية
```

### إضافة تحديث

من صفحة القضية:

```text
Timeline
 ↓
إضافة تحديث
 ↓
Title
 ↓
Description
 ↓
IsImportant
 ↓
Save
```

---

# 29. Scenario 6 — السكرتير / الموظف الإداري

المهام اليومية:

```text
تسجيل الموكلين
حجز الاستشارات
تنظيم الجلسات
رفع المستندات
إدخال بيانات القضايا
متابعة التوكيلات
تسجيل المدفوعات حسب الصلاحية
إصدار الفواتير حسب الصلاحية
إنشاء المهام
متابعة التنبيهات
```

السكرتير لا يستطيع الوصول تلقائيًا لكل بيانات النظام؛ الوصول يتم حسب Permissions.

---

# 30. Scenario 7 — صاحب المكتب / الشريك

Dashboard الإدارة:

```text
القضايا المفتوحة
القضايا المغلقة
الجلسات القادمة
القضايا لكل محامي
التحصيل الشهري
المتأخرات
المصروفات
صافي الإيرادات
```

## أداء المحامين

يتم عرض:

```text
عدد القضايا
عدد الجلسات
المهام المكتملة
المهام المتأخرة
القضايا المغلقة
التحصيل المرتبط بالقضايا
```

## التقارير

يستطيع صاحب المكتب استخراج:

```text
PDF
Excel
```

وفق الفترة:

```text
اليوم
الأسبوع
الشهر
السنة
Custom Range
```

---

# 31. Scenario 8 — Client Portal

إذا تم تفعيل Client Portal، يجب فصل صلاحيات الموكل عن موظفي المكتب.

الموكل يستطيع رؤية:

```text
ملفه الشخصي
القضايا المسموح له بها
الجلسات
المواعيد
المستندات المسموح بها
الفواتير
المدفوعات
المبالغ المستحقة
الإشعارات
```

لا يستطيع:

```text
تعديل بيانات القضية القانونية
رؤية ملاحظات داخلية
رؤية Audit Logs
رؤية بيانات محامين آخرين
الوصول لمستندات غير مصرح بها
```

---

# 32. Data Relationships

العلاقة الأساسية:

```text
Client
 │
 ├── Consultations
 │
 ├── PowerOfAttorney
 │
 ├── FeeAgreements
 │      │
 │      └── Invoices
 │              │
 │              └── Payments
 │
 └── Cases
       │
       ├── CaseParties
       ├── CaseAssignments → User
       ├── Hearings
       ├── CaseTimeline
       ├── Documents
       │      └── DocumentVersions
       ├── Tasks
       ├── PowerOfAttorney
       ├── FeeAgreement
       ├── Payments
       └── Expenses
```

---

# 33. Automatic Events and Notifications

## Client Created

```text
AuditLog
```

## Consultation Created

```text
Notification
```

## Case Created

```text
Timeline
AuditLog
Notification to assigned users
```

## Lawyer Assigned

```text
In-App Notification
AuditLog
Timeline
```

## Hearing Created

```text
Schedule Notifications
```

## Hearing Rescheduled

```text
Cancel old schedules
Create new schedules
Notify affected users
AuditLog
```

## Hearing Completed

```text
Timeline
AuditLog
Optional notification
```

## Next Hearing Created

```text
Schedule Notifications
```

## Power of Attorney Near Expiry

```text
Notification
Email
Optional WhatsApp
```

## Invoice Overdue

```text
Notification
Email
Optional WhatsApp
```

## Task Due

```text
Notification
```

---

# 34. SignalR

يستخدم SignalR لتحديث الواجهة فورًا.

أمثلة:

```text
NotificationHub
```

عند وصول Notification:

```text
Server
 ↓
SignalR
 ↓
Connected User
 ↓
Update Notification Badge
```

لا يتم استخدام SignalR كبديل دائم للتخزين.

الـ Notification يتم حفظه أولًا في DB ثم يتم Push عبر SignalR.

---

# 35. RTL and Arabic UI

النظام عربي بالكامل.

يجب أن يكون:

```html
<html lang="ar" dir="rtl">
```

ويجب ضبط MudBlazor للعمل مع RTL.

المصطلحات الأساسية:

```text
Client → الموكل
Case → القضية
Hearing → الجلسة
Power of Attorney → التوكيل
Consultation → الاستشارة
Fee Agreement → عقد الأتعاب
Payment → الدفعة
Expense → المصروف
Invoice → الفاتورة
Document → المستند
Task → المهمة
Timeline → سجل القضية
Audit Log → سجل التدقيق
```

---

# 36. File Storage

كل الملفات تستخدم:

```csharp
IFileService
```

ولا يتم وضع File System logic داخل Domain.

يجب دعم:

```text
Upload
Download
Delete
Versioning
Validation
File Size Limits
Allowed Extensions
```

ويجب منع رفع الملفات الخطرة أو غير المسموح بها.

---

# 37. Security

يجب تطبيق:

```text
Authentication
Authorization
Permissions
Anti-forgery
Input Validation
File Validation
Audit Logging
Secure Password Policy
Session Security
```

ويجب عدم الاعتماد على إخفاء زر في UI كحماية.

مثال:

```text
UI hides Delete button
+
Application verifies permission
+
Server verifies authorization
```

---

# 38. Naming Conventions

| النوع | Convention |
|---|---|
| Entity | PascalCase |
| Entity | `sealed class` |
| Command | `{Action}{Entity}Command` |
| Handler | `{Action}{Entity}CommandHandler` |
| Validator | `{Action}{Entity}CommandValidator` |
| Query | `Get{Entity}{Criteria}Query` |
| Response | `{Entity}Response` |
| Repository | `I{Entity}Repository` |
| Configuration | `{Entity}Configuration` |
| Domain Event | `{Entity}{Action}DomainEvent` |
| Integration Event | `{Entity}{Action}IntegrationEvent` |

---

# 39. Error Convention

الأخطاء بالعربية:

```csharp
public static class ClientErrors
{
    public static readonly Error NameRequired =
        new("Client.NameRequired", "اسم الموكل مطلوب.");

    public static readonly Error DuplicateNationalId =
        new("Client.DuplicateNationalId", "رقم الهوية مستخدم بالفعل.");
}
```

صيغة Error Code:

```text
{Entity}.{ErrorName}
```

---

# 40. Domain Events

أمثلة:

```text
ClientCreatedDomainEvent
CaseCreatedDomainEvent
CaseAssignedDomainEvent
HearingCreatedDomainEvent
HearingRescheduledDomainEvent
HearingCompletedDomainEvent
PaymentCreatedDomainEvent
InvoiceIssuedDomainEvent
DocumentUploadedDomainEvent
TaskAssignedDomainEvent
```

يتم نشر Domain Events بعد نجاح عملية الحفظ.

---

# 41. Dependency Injection

## Application

```csharp
builder.Services.AddLegalPracticeApplication();
```

## Infrastructure

```csharp
builder.Services.AddLegalPracticeInfrastructure(
    builder.Configuration);
```

## Web

```csharp
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
```

ويتم تسجيل:

```text
Identity
Authorization
MudBlazor
SignalR
QuestPDF
EF Core
Repositories
File Services
Notification Services
Audit Services
```

---

# 42. Program.cs

التسجيل المنطقي:

```text
AddSharedApplication()
AddSharedInfrastructure()

AddLegalPracticeApplication()
AddLegalPracticeInfrastructure()

AddIdentity()
AddAuthorization()

AddMudBlazor()
AddSignalR()

AddRazorComponents()
    .AddInteractiveServerComponents()
```

---

# 43. How to Add a New Entity

1. إنشاء Entity داخل المجال المناسب في `LegalPractice.Domain`.
2. إضافة Business Rules.
3. إنشاء Errors.
4. إنشاء Domain Events عند الحاجة.
5. إضافة DbSet إلى `LegalPracticeDbContext`.
6. إنشاء EF Configuration.
7. إضافة Repository فقط إذا كان Base Repository غير كافٍ.
8. إنشاء Commands.
9. إنشاء Queries.
10. إنشاء Validators.
11. إضافة Permissions.
12. إنشاء صفحات Blazor.
13. إضافة Audit behavior.
14. إضافة Notifications عند الحاجة.
15. إنشاء Migration.
16. اختبار السيناريو كاملًا.

---

# 44. How to Add a New Use Case

مثال: تسجيل دفعة.

```text
Application/
└── Finance/
    └── Payments/
        └── Commands/
            └── CreatePayment/
                ├── CreatePaymentCommand.cs
                ├── CreatePaymentCommandHandler.cs
                └── CreatePaymentCommandValidator.cs
```

الـ Handler مسؤول عن orchestration.

لكن Business Rules الأساسية يجب أن تبقى في Domain.

---

# 45. Important Architectural Rules

## ممنوع

```text
Blazor Page
   ↓
DbContext مباشرة
```

## الصحيح

```text
Blazor Page
   ↓
ISender
   ↓
Command / Query
   ↓
Handler
   ↓
Domain / Repository
   ↓
UnitOfWork
   ↓
DbContext
```

---

## ممنوع

وضع business rules داخل Razor Components.

## الصحيح

```text
Domain
```

هو المكان الأساسي لقواعد العمل.

---

## ممنوع

ربط Domain بـ:

```text
MudBlazor
EF Core
SignalR
QuestPDF
ASP.NET Identity
```

---

# 46. Recommended Development Order

ترتيب التنفيذ:

```text
1. Shared Foundation
2. Identity + Permissions
3. Client
4. Case
5. Case Parties
6. Case Assignment
7. Power of Attorney
8. Documents
9. Hearings + Calendar
10. Timeline
11. Tasks
12. Fee Agreements
13. Invoices
14. Payments
15. Expenses
16. Notifications
17. Dashboard
18. Reports
19. Audit Log
20. Global Search
21. Client Portal
```

---

# 47. End-to-End Business Flow

المسار الطبيعي للمكتب:

```text
موكل
  ↓
استشارة (اختياري)
  ↓
توكيل
  ↓
فتح قضية
  ↓
إضافة الأطراف
  ↓
توزيع القضية
  ↓
رفع المستندات
  ↓
عقد الأتعاب
  ↓
الفاتورة
  ↓
الدفعات
  ↓
الجلسات
  ↓
Timeline
  ↓
المهام
  ↓
الجلسة القادمة
  ↓
التنبيهات
  ↓
إغلاق القضية
  ↓
الأرشفة
```

---

# 48. Agent Instructions

عند تنفيذ أي طلب تطوير:

1. اقرأ هذه الوثيقة أولًا.
2. لا تنشئ Module Project جديدًا.
3. لا تنشئ Domain/Application/Infrastructure مستقلًا لكل مجال.
4. ضع الكود داخل المشروع المناسب والمجلد المناسب.
5. حافظ على Clean Architecture.
6. حافظ على DDD في Domain.
7. استخدم CQRS في Application.
8. لا تضع Business Logic داخل Blazor.
9. استخدم EF Core في Infrastructure.
10. استخدم Identity + Permissions.
11. أي عملية حساسة يجب أن تدعم Audit.
12. الحذف الافتراضي Soft Delete.
13. الملفات تستخدم `IFileService`.
14. التنبيهات تستخدم abstractions ولا تربط Domain بمزود خارجي.
15. كل UI يجب أن يدعم RTL والعربية.
16. أي Feature جديد يجب أن يحدد:
    - Entity changes
    - Commands
    - Queries
    - Validation
    - Permissions
    - Audit
    - Notifications
    - UI
    - Database migration
17. لا تكسر العلاقات الحالية عند إضافة Feature.
18. لا تضف abstraction غير ضروري.
19. لا تستخدم Repository مخصصًا إذا كان Base Repository كافيًا.
20. أي قرار معماري جديد يجب أن يحافظ على كون النظام **Single Business Module + Shared Layers**.

---

# 49. Final Architecture

```text
                    ┌─────────────────────────────┐
                    │     LegalPractice.Web       │
                    │ Blazor + MudBlazor + RTL    │
                    │ Interactive Server + SignalR│
                    └──────────────┬──────────────┘
                                   │
                    ┌──────────────▼──────────────┐
                    │   LegalPractice.Application │
                    │ CQRS + MediatR + Validation │
                    │ Use Cases + DTOs             │
                    └──────────────┬──────────────┘
                                   │
                    ┌──────────────▼──────────────┐
                    │      LegalPractice.Domain   │
                    │ Clients / Cases / Finance   │
                    │ Hearings / Documents / etc. │
                    │ Business Rules + Events     │
                    └──────────────┬──────────────┘
                                   │
                    ┌──────────────▼──────────────┐
                    │ LegalPractice.Infrastructure│
                    │ EF Core + SQL Server        │
                    │ Identity + Files + Audit    │
                    │ Notifications + Reports     │
                    └──────────────┬──────────────┘
                                   │
                    ┌──────────────▼──────────────┐
                    │       Shared Layers          │
                    │ Domain / Application / Infra │
                    └─────────────────────────────┘
```

**القاعدة النهائية:** النظام Business Module واحد، لكنه منظم داخليًا إلى مجالات واضحة. لا يتم تحويل كل مجال إلى Project أو Module مستقل إلا إذا ظهرت لاحقًا حاجة حقيقية لذلك.
