namespace App.Domain.Common;

/// <summary>
/// واجهة التدقيق — تطبق على الكيانات التي تحتاج تتبع الإنشاء والتعديل
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; }
    string CreatedBy { get; }
    DateTime? UpdatedAt { get; }
    string? UpdatedBy { get; }
    void SetCreated(string createdBy);
    void SetUpdated(string updatedBy);
}
