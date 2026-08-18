using Shared.Application.Messaging;

namespace App.Application.PowerOfAttorney.Commands.CreatePowerOfAttorney;

public record CreatePowerOfAttorneyCommand(
    Guid ClientId,
    Guid? CaseId,
    string PowerNumber,
    DateTime IssueDate,
    DateTime? ExpiryDate,
    string? NotaryName,
    string? NotaryNumber,
    string? FilePath,
    string? Notes,
    string CreatedBy) : ICommand<Guid>;
