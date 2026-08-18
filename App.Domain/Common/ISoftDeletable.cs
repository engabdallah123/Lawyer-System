namespace App.Domain.Common;

/// <summary>
/// واجهة الحذف المنطقي — تطبق على الكيانات التي تدعم Soft Delete
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    string? DeletedBy { get; }
    void SoftDelete(string deletedBy);
    void Restore();
}
