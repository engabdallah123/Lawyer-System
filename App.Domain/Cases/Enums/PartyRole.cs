namespace App.Domain.Cases.Enums;

/// <summary>
/// دور وصفة الطرف في القضية القانونية
/// </summary>
public enum PartyRole
{
    /// <summary>مدعي</summary>
    Plaintiff = 0,

    /// <summary>مدعى عليه</summary>
    Defendant = 1,

    /// <summary>مستأنف</summary>
    Appellant = 2,

    /// <summary>مستأنف ضده</summary>
    Appellee = 3,

    /// <summary>طاعن (نقض / تمييز)</summary>
    Petitioner = 4,

    /// <summary>مطعون ضده</summary>
    Respondent = 5,

    /// <summary>متهم</summary>
    Accused = 6,

    /// <summary>مجني عليه / شاكي</summary>
    Victim = 7,

    /// <summary>شاهد</summary>
    Witness = 8,

    /// <summary>خبير معتمد</summary>
    Expert = 9,

    /// <summary>شريك / متدخل هجومي أو انضمامي</summary>
    Partner = 10,

    /// <summary>دائن</summary>
    Creditor = 11,

    /// <summary>مدين</summary>
    Debtor = 12,

    /// <summary>محامي الطرف الآخر</summary>
    OtherLawyer = 13,

    /// <summary>أخرى</summary>
    Other = 14
}

public static class PartyRoleExtensions
{
    public static string GetArabicName(this PartyRole role) => role switch
    {
        PartyRole.Plaintiff => "مدعي",
        PartyRole.Defendant => "مدعى عليه",
        PartyRole.Appellant => "مستأنف",
        PartyRole.Appellee => "مستأنف ضده",
        PartyRole.Petitioner => "طاعن",
        PartyRole.Respondent => "مطعون ضده",
        PartyRole.Accused => "متهم",
        PartyRole.Victim => "مجني عليه / شاكي",
        PartyRole.Witness => "شاهد",
        PartyRole.Expert => "خبير",
        PartyRole.Partner => "خصم متدخل / شريك",
        PartyRole.Creditor => "دائن",
        PartyRole.Debtor => "مدين",
        PartyRole.OtherLawyer => "محامي الطرف الآخر",
        PartyRole.Other => "أخرى / صفة إجرائية",
        _ => role.ToString()
    };
}
