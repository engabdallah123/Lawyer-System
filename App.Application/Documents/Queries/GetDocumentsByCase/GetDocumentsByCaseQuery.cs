using App.Application.Documents.DTOs;
using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Documents.Queries.GetDocumentsByCase
{
    public record GetDocumentsByCaseQuery(Guid CaseId) : IQuery<IEnumerable<DocumentDto>>;
}
