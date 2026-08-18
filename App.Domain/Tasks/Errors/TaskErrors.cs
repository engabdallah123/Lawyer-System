using Shared.Domain;

namespace App.Domain.Tasks.Errors;

public static class TaskErrors
{
    public static readonly Error TitleRequired =
        new("LegalTask.TitleRequired", "عنوان المهمة مطلوب.");

    public static readonly Error AssignedUserRequired =
        new("LegalTask.AssignedUserRequired", "يجب تحديد المستخدم المسند إليه المهمة.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("LegalTask.NotFound", $"المهمة ذات المعرف '{id}' غير موجودة.");
}
