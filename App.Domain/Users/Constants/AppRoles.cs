namespace App.Domain.Users.Constants;

public static class AppRoles
{
    public const string Administrator = "Administrator";
    public const string Lawyer = "Lawyer";
    public const string Staff = "Staff";

    public static readonly IReadOnlyList<string> All =
    [
        Administrator,
        Lawyer,
        Staff
    ];

    public static string GetArabicName(string role) => role switch
    {
        Administrator => "مدير النظام (كامل الصلاحيات)",
        Lawyer => "محامي / مستشار قانوني",
        Staff => "موظف / سكرتارية",
        _ => role
    };
}
