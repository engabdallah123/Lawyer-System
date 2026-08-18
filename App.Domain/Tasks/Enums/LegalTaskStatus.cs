namespace App.Domain.Tasks.Enums;

/// <summary>
/// حالة المهمة
/// </summary>
public enum LegalTaskStatus
{
    /// <summary>معلقة</summary>
    Pending = 0,

    /// <summary>قيد التنفيذ</summary>
    InProgress = 1,

    /// <summary>مكتملة</summary>
    Completed = 2,

    /// <summary>ملغاة</summary>
    Cancelled = 3
}
