using Shared.Application.Messaging;

namespace App.Application.Finance.Commands.IssueInvoice;

public record IssueInvoiceCommand(Guid InvoiceId, string UpdatedBy) : ICommand;
