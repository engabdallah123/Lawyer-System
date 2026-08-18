namespace App.Domain.Cases.Enums;

/// <summary>
/// دور الطرف في القضية
/// </summary>
public enum PartyRole
{
    /// <summary>مدعي</summary>
    Plaintiff = 0,

    /// <summary>مدعى عليه</summary>
    Defendant = 1,

    /// <summary>شاهد</summary>
    Witness = 2,

    /// <summary>محامي الطرف الآخر</summary>
    OtherLawyer = 3,

    /// <summary>أخرى</summary>
    Other = 4
}
