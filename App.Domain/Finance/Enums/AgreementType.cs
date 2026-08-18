namespace App.Domain.Finance.Enums;

/// <summary>
/// نوع عقد الأتعاب
/// </summary>
public enum AgreementType
{
    /// <summary>مبلغ ثابت</summary>
    Fixed = 0,

    /// <summary>نسبة مئوية</summary>
    Percentage = 1,

    /// <summary>بالساعة</summary>
    Hourly = 2
}
