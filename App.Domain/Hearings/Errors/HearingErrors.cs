using Shared.Domain;

namespace App.Domain.Hearings.Errors;

public static class HearingErrors
{
    public static readonly Error HearingTypeRequired =
        new("Hearing.TypeRequired", "نوع الجلسة مطلوب.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("Hearing.NotFound", $"الجلسة ذات المعرف '{id}' غير موجودة.");

    public static readonly Error DateInPast =
        new("Hearing.DateInPast", "لا يمكن تحديد جلسة بتاريخ ماضٍ.");
}
