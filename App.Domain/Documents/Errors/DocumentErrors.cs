using Shared.Domain;

namespace App.Domain.Documents.Errors;

public static class DocumentErrors
{
    public static readonly Error NameRequired =
        new("Document.NameRequired", "اسم المستند مطلوب.");

    public static readonly Error FilePathRequired =
        new("DocumentVersion.FilePathRequired", "مسار الملف مطلوب.");

    public static readonly Error FileNameRequired =
        new("DocumentVersion.FileNameRequired", "اسم الملف مطلوب.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("Document.NotFound", $"المستند ذو المعرف '{id}' غير موجود.");
}
