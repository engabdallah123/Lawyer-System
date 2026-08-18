using Shared.Domain;

namespace App.Domain.PowerOfAttorney.Errors;

public static class PowerOfAttorneyErrors
{
    public static readonly Error PowerNumberRequired =
        new("PowerOfAttorney.NumberRequired", "رقم التوكيل مطلوب.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("PowerOfAttorney.NotFound", $"التوكيل ذو المعرف '{id}' غير موجود.");

    public static readonly Error Expired =
        new("PowerOfAttorney.Expired", "التوكيل منتهي الصلاحية.");
}
