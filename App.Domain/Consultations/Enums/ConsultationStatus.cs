namespace App.Domain.Consultations.Enums;

/// <summary>
/// حالة الاستشارة
/// </summary>
public enum ConsultationStatus
{
    /// <summary>مجدولة</summary>
    Scheduled = 0,

    /// <summary>مكتملة</summary>
    Completed = 1,

    /// <summary>ملغاة</summary>
    Cancelled = 2
}
