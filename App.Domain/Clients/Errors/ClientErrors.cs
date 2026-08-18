using Shared.Domain;

namespace App.Domain.Clients.Errors;

/// <summary>
/// أخطاء الموكل
/// </summary>
public static class ClientErrors
{
    public static readonly Error FullNameRequired =
        new("Client.FullNameRequired", "اسم الموكل مطلوب للأشخاص الطبيعيين.");

    public static readonly Error CompanyNameRequired =
        new("Client.CompanyNameRequired", "اسم الشركة مطلوب للشركات.");

    public static readonly Error PhoneRequired =
        new("Client.PhoneRequired", "رقم الهاتف الأساسي مطلوب.");

    public static readonly Error DuplicateNationalId =
        new("Client.DuplicateNationalId", "رقم الهوية مستخدم بالفعل.");

    public static readonly Error DuplicateCommercialRegister =
        new("Client.DuplicateCommercialRegister", "رقم السجل التجاري مستخدم بالفعل.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("Client.NotFound", $"الموكل ذو المعرف '{id}' غير موجود.");

    public static readonly Error HasRelatedData =
        new("Client.HasRelatedData", "لا يمكن حذف الموكل لوجود بيانات قانونية مرتبطة به.");
}
