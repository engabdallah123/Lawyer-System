using App.Domain.Cases.Enums;
using Shared.Domain;

namespace App.Domain.Cases.Entities;

/// <summary>
/// طرف في القضية — يربط القضية بموكل أو باسم حر
/// </summary>
public sealed class CaseParty : Entity
{
    public Guid CaseId { get; private set; }
    public Guid? ClientId { get; private set; }
    public string? PartyName { get; private set; }
    public PartyRole PartyRole { get; private set; }
    public bool IsMainClient { get; private set; }
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
        PartyRole partyRole,
        bool isMainClient,
        string? notes)
        : base(id)
    {
        CaseId = caseId;
        ClientId = clientId;
        PartyName = partyName;
        PartyRole = partyRole;
        IsMainClient = isMainClient;
        Notes = notes;
    }

    public static Result<CaseParty> Create(
        Guid caseId,
        Guid? clientId,
        string? partyName,
        PartyRole partyRole,
        bool isMainClient,
        string? notes)
    {
        // يجب أن يكون هناك موكل أو اسم حر
        if (clientId == null && string.IsNullOrWhiteSpace(partyName))
            return Result<CaseParty>.Failure(Errors.CaseErrors.PartyNameOrClientRequired);

        var party = new CaseParty(
            Guid.NewGuid(),
            caseId,
            clientId,
            partyName?.Trim(),
            partyRole,
            isMainClient,
            notes?.Trim());

        return Result<CaseParty>.Success(party);
    }

    public Result Update(
        Guid? clientId,
        string? partyName,
        PartyRole partyRole,
        bool isMainClient,
        string? notes)
    {
        if (clientId == null && string.IsNullOrWhiteSpace(partyName))
            return Result.Failure(Errors.CaseErrors.PartyNameOrClientRequired);

        ClientId = clientId;
        PartyName = partyName?.Trim();
        PartyRole = partyRole;
        IsMainClient = isMainClient;
        Notes = notes?.Trim();

        return Result.Success();
    }
}
