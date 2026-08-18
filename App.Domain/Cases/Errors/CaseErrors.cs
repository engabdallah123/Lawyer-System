using Shared.Domain;

namespace App.Domain.Cases.Errors;

/// <summary>
/// أخطاء القضايا
/// </summary>
public static class CaseErrors
{
    public static readonly Error InternalNumberRequired =
        new("Case.InternalNumberRequired", "الرقم الداخلي للقضية مطلوب.");

    public static readonly Error TitleRequired =
        new("Case.TitleRequired", "عنوان القضية مطلوب.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("Case.NotFound", $"القضية ذات المعرف '{id}' غير موجودة.");

    public static readonly Error PartyNameOrClientRequired =
        new("CaseParty.NameOrClientRequired", "يجب تحديد الموكل أو إدخال اسم الطرف.");

    public static readonly Error UserIdRequired =
        new("CaseAssignment.UserIdRequired", "يجب تحديد المحامي المسند إليه.");

    public static readonly Error RoleInCaseRequired =
        new("CaseAssignment.RoleRequired", "يجب تحديد دور المحامي في القضية.");

    public static readonly Error TimelineTitleRequired =
        new("CaseTimeline.TitleRequired", "عنوان الحدث مطلوب.");

    public static readonly Error AlreadyClosed =
        new("Case.AlreadyClosed", "القضية مغلقة بالفعل.");
}
