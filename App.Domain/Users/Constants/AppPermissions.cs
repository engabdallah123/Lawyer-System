namespace App.Domain.Users.Constants;

public static class AppPermissions
{
    // Clients
    public const string ClientsView = "Permissions.Clients.View";
    public const string ClientsCreate = "Permissions.Clients.Create";
    public const string ClientsEdit = "Permissions.Clients.Edit";
    public const string ClientsDelete = "Permissions.Clients.Delete";

    // Cases
    public const string CasesView = "Permissions.Cases.View";
    public const string CasesCreate = "Permissions.Cases.Create";
    public const string CasesEdit = "Permissions.Cases.Edit";
    public const string CasesDelete = "Permissions.Cases.Delete";
    public const string CasesAssign = "Permissions.Cases.Assign";

    // Hearings
    public const string HearingsView = "Permissions.Hearings.View";
    public const string HearingsCreate = "Permissions.Hearings.Create";
    public const string HearingsEdit = "Permissions.Hearings.Edit";
    public const string HearingsDelete = "Permissions.Hearings.Delete";

    // Consultations
    public const string ConsultationsView = "Permissions.Consultations.View";
    public const string ConsultationsCreate = "Permissions.Consultations.Create";
    public const string ConsultationsEdit = "Permissions.Consultations.Edit";

    // Power of Attorney
    public const string PowerOfAttorneyView = "Permissions.PowerOfAttorney.View";
    public const string PowerOfAttorneyCreate = "Permissions.PowerOfAttorney.Create";
    public const string PowerOfAttorneyEdit = "Permissions.PowerOfAttorney.Edit";

    // Finance
    public const string FinanceView = "Permissions.Finance.View";
    public const string FinanceManage = "Permissions.Finance.Manage";
    public const string InvoicesCreate = "Permissions.Invoices.Create";
    public const string PaymentsCreate = "Permissions.Payments.Create";
    public const string ExpensesCreate = "Permissions.Expenses.Create";

    // Documents
    public const string DocumentsView = "Permissions.Documents.View";
    public const string DocumentsUpload = "Permissions.Documents.Upload";
    public const string DocumentsDelete = "Permissions.Documents.Delete";

    // Tasks
    public const string TasksView = "Permissions.Tasks.View";
    public const string TasksManage = "Permissions.Tasks.Manage";

    // Reports
    public const string ReportsView = "Permissions.Reports.View";
    public const string ReportsExport = "Permissions.Reports.Export";

    // Users & Roles
    public const string UsersView = "Permissions.Users.View";
    public const string UsersManage = "Permissions.Users.Manage";
    public const string PermissionsManage = "Permissions.Permissions.Manage";

    // Audit
    public const string AuditView = "Permissions.Audit.View";

    public record PermissionGroup(string GroupName, List<PermissionItem> Permissions);
    public record PermissionItem(string Value, string DisplayName);

    public static List<PermissionGroup> GetAllGroups() =>
    [
        new("إدارة الموكلين",
        [
            new(ClientsView, "عرض الموكلين"),
            new(ClientsCreate, "إضافة موكل"),
            new(ClientsEdit, "تعديل بيانات موكل"),
            new(ClientsDelete, "حذف موكل")
        ]),
        new("إدارة القضايا والدعاوى",
        [
            new(CasesView, "عرض القضايا"),
            new(CasesCreate, "إنشاء قضية جديدة"),
            new(CasesEdit, "تعديل تفاصيل القضية"),
            new(CasesDelete, "حذف وأرشفة القضية"),
            new(CasesAssign, "تعيين وإسناد المحامين")
        ]),
        new("جلسات المحاكم",
        [
            new(HearingsView, "عرض الجلسات والمواعيد"),
            new(HearingsCreate, "جدولة جلسة جديدة"),
            new(HearingsEdit, "تسجيل وتعديل قرارات الجلسات"),
            new(HearingsDelete, "إلغاء جلسة")
        ]),
        new("الاستشارات والتوكيلات",
        [
            new(ConsultationsView, "عرض الاستشارات القانونية"),
            new(ConsultationsCreate, "تسجيل استشارة جديدة"),
            new(ConsultationsEdit, "تعديل الاستشارات"),
            new(PowerOfAttorneyView, "عرض التوكيلات"),
            new(PowerOfAttorneyCreate, "إضافة توكيل جديد"),
            new(PowerOfAttorneyEdit, "تعديل التوكيل")
        ]),
        new("الشؤون المالية والتحصيل",
        [
            new(FinanceView, "عرض السجلات المالية والتقارير"),
            new(FinanceManage, "إدارة عقود الأتعاب"),
            new(InvoicesCreate, "إصدار وإلغاء الفواتير"),
            new(PaymentsCreate, "تسجيل سندات القبض والمدفوعات"),
            new(ExpensesCreate, "تسجيل المصروفات القضائية")
        ]),
        new("المستندات والمهام",
        [
            new(DocumentsView, "استعراض المستندات والملفات"),
            new(DocumentsUpload, "رفع وتحديث نسخ المستندات"),
            new(DocumentsDelete, "حذف المستندات"),
            new(TasksView, "عرض المهام والمتابعات"),
            new(TasksManage, "إنشاء وتعيين وإغلاق المهام")
        ]),
        new("التقارير وسجل التدقيق",
        [
            new(ReportsView, "عرض التقارير التشغيلية والمالية"),
            new(ReportsExport, "تصدير التقارير (PDF / Excel)"),
            new(AuditView, "الاطلاع على سجل التدقيق والرقابة (Audit Log)")
        ]),
        new("المستخدمون والأمان",
        [
            new(UsersView, "عرض قائمة المستخدمين"),
            new(UsersManage, "إضافة وتعديل وحظر المستخدمين"),
            new(PermissionsManage, "إدارة وتعديل الصلاحيات والأدوار")
        ])
    ];

    public static List<string> GetAllPermissions() =>
        GetAllGroups().SelectMany(g => g.Permissions.Select(p => p.Value)).ToList();

    public static List<string> GetDefaultPermissionsForRole(string role) => role switch
    {
        AppRoles.Administrator => GetAllPermissions(),
        AppRoles.Lawyer =>
        [
            ClientsView, ClientsCreate, ClientsEdit,
            CasesView, CasesCreate, CasesEdit, CasesAssign,
            HearingsView, HearingsCreate, HearingsEdit,
            ConsultationsView, ConsultationsCreate, ConsultationsEdit,
            PowerOfAttorneyView, PowerOfAttorneyCreate, PowerOfAttorneyEdit,
            FinanceView, InvoicesCreate, PaymentsCreate, ExpensesCreate,
            DocumentsView, DocumentsUpload,
            TasksView, TasksManage,
            ReportsView, ReportsExport
        ],
        AppRoles.Staff =>
        [
            ClientsView, ClientsCreate,
            CasesView,
            HearingsView,
            ConsultationsView,
            PowerOfAttorneyView,
            DocumentsView, DocumentsUpload,
            TasksView,
            ReportsView
        ],
        _ => []
    };
}
