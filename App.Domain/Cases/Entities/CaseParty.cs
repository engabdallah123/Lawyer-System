using App.Domain.Cases.Enums;
using Shared.Domain;

namespace App.Domain.Cases.Entities;

/// <summary>
/// طرف في القضية — يربط القضية بموكل أو بخصم/طرف خارجي مع بيانات الاتصال ومحاميه
/// </summary>
public sealed class CaseParty : Entity
{
    public Guid CaseId { get; private set; }
    public Guid? ClientId { get; private set; }
    public string? PartyName { get; private set; }
    public string? PartyType { get; private set; } // فرد / شركة / جهة حكومية / هيئة
    public PartyRole PartyRole { get; private set; }
    public bool IsMainClient { get; private set; }
    public string? Address { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? LawyerName { get; private set; }
    public string? LawyerPhone { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Properties
    public Case Case { get; private set; } = null!;
    public Clients.Entities.Client? Client { get; private set; }

    // EF Core Constructor
    private CaseParty() { }

    private CaseParty(
        Guid id,
        Guid caseId,
        Guid? clientId,
        string? partyName,
        string? partyType,
        PartyRole partyRole,
        bool isMainClient,
        string? address,
        string? phone,
        string? email,
        string? lawyerName,
        string? lawyerPhone,
        string? notes)
        : base(id)
    {
        CaseId = caseId;
        ClientId = clientId;
        PartyName = partyName;
        PartyType = partyType;
        PartyRole = partyRole;
        IsMainClient = isMainClient;
        Address = address;
        Phone = phone;
        Email = email;
        LawyerName = lawyerName;
        LawyerPhone = lawyerPhone;
        Notes = notes;
    }

    public static Result<CaseParty> Create(
        Guid caseId,
        Guid? clientId,
        string? partyName,
        string? partyType,
        PartyRole partyRole,
        bool isMainClient,
        string? address = null,
        string? phone = null,
        string? email = null,
        string? lawyerName = null,
        string? lawyerPhone = null,
        string? notes = null)
    {
        // يجب أن يكون هناك موكل مسجل أو اسم للطرف الخارجي
        if (clientId == null && string.IsNullOrWhiteSpace(partyName))
            return Result<CaseParty>.Failure(Errors.CaseErrors.PartyNameOrClientRequired);

        var party = new CaseParty(
            Guid.NewGuid(),
            caseId,
            clientId,
            partyName?.Trim(),
            partyType?.Trim(),
            partyRole,
            isMainClient,
            address?.Trim(),
            phone?.Trim(),
            email?.Trim(),
            lawyerName?.Trim(),
            lawyerPhone?.Trim(),
            notes?.Trim());

        return Result<CaseParty>.Success(party);
    }

    public Result Update(
        Guid? clientId,
        string? partyName,
        string? partyType,
        PartyRole partyRole,
        bool isMainClient,
        string? address = null,
        string? phone = null,
        string? email = null,
        string? lawyerName = null,
        string? lawyerPhone = null,
        string? notes = null)
    {
        if (clientId == null && string.IsNullOrWhiteSpace(partyName))
            return Result.Failure(Errors.CaseErrors.PartyNameOrClientRequired);

        ClientId = clientId;
        PartyName = partyName?.Trim();
        PartyType = partyType?.Trim();
        PartyRole = partyRole;
        IsMainClient = isMainClient;
        Address = address?.Trim();
        Phone = phone?.Trim();
        Email = email?.Trim();
        LawyerName = lawyerName?.Trim();
        LawyerPhone = lawyerPhone?.Trim();
        Notes = notes?.Trim();

        return Result.Success();
    }
}
