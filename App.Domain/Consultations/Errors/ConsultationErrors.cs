using Shared.Domain;

namespace App.Domain.Consultations.Errors;

public static class ConsultationErrors
{
    public static readonly Error SubjectRequired =
        new("Consultation.SubjectRequired", "موضوع الاستشارة مطلوب.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("Consultation.NotFound", $"الاستشارة ذات المعرف '{id}' غير موجودة.");
}
