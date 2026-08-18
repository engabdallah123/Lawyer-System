namespace App.Domain.Tasks.Enums;

/// <summary>
/// أولوية المهمة
/// </summary>
public enum TaskPriority
{
    /// <summary>منخفضة</summary>
    Low = 0,

    /// <summary>عادية</summary>
    Normal = 1,

    /// <summary>عالية</summary>
    High = 2,

    /// <summary>عاجلة</summary>
    Urgent = 3
}
