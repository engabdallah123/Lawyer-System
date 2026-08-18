using App.Domain.Finance.Enums;
using Shared.Application.Messaging;

namespace App.Application.Finance.Commands.CreateFeeAgreement;

public record CreateFeeAgreementCommand(
    Guid ClientId,
    Guid? CaseId,
    AgreementType AgreementType,
    decimal TotalAmount,
    string? Description,
    DateTime StartDate,
    DateTime? EndDate,
    string CreatedBy) : ICommand<Guid>;
