using Shared.Application.Messaging;

namespace App.Application.PowerOfAttorney.Commands.UpdatePowerOfAttorney;

public record UpdatePowerOfAttorneyCommand(
    Guid Id,
    Guid? CaseId,
    string PowerNumber,
    DateTime IssueDate,
    DateTime? ExpiryDate,
    string? NotaryName,
    string? NotaryNumber,
    string? Notes,
    string UpdatedBy) : ICommand;
