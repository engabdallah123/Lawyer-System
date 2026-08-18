using App.Domain.Common;
using Shared.Domain;

namespace App.Domain.Documents.Entities;

/// <summary>
/// المستند — metadata فقط، الملفات الفعلية في DocumentVersion
/// </summary>
public sealed class Document : Entity, ISoftDeletable, IAuditable
{
    public Guid? CaseId { get; private set; }
    public Guid? ClientId { get; private set; }
    public int? DocumentTypeId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid? CurrentVersionId { get; private set; }

    // ISoftDeletable
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    // IAuditable
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Navigation Properties
    public Lookups.DocumentType? DocumentType { get; private set; }
    public Cases.Entities.Case? Case { get; private set; }
    public Clients.Entities.Client? Client { get; private set; }
    public DocumentVersion? CurrentVersion { get; private set; }
    public ICollection<DocumentVersion> DocumentVersions { get; private set; } = new List<DocumentVersion>();

    private Document() { }

    private Document(Guid id, Guid? caseId, Guid? clientId, int? documentTypeId, string name, string? description)
        : base(id)
    {
        CaseId = caseId;
        ClientId = clientId;
        DocumentTypeId = documentTypeId;
        Name = name;
        Description = description;
        IsDeleted = false;
    }

    public static Result<Document> Create(
        Guid? caseId, Guid? clientId, int? documentTypeId, string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Document>.Failure(Errors.DocumentErrors.NameRequired);

        var doc = new Document(Guid.NewGuid(), caseId, clientId, documentTypeId, name.Trim(), description?.Trim());
        return Result<Document>.Success(doc);
    }

    public void SetCurrentVersion(Guid versionId) => CurrentVersionId = versionId;

    public Result Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Errors.DocumentErrors.NameRequired);

        Name = name.Trim();
        Description = description?.Trim();
        return Result.Success();
    }

    // ISoftDeletable
    public void SoftDelete(string deletedBy) { IsDeleted = true; DeletedAt = DateTime.UtcNow; DeletedBy = deletedBy; }
    public void Restore() { IsDeleted = false; DeletedAt = null; DeletedBy = null; }

    // IAuditable
    public void SetCreated(string createdBy) { CreatedAt = DateTime.UtcNow; CreatedBy = createdBy; }
    public void SetUpdated(string updatedBy) { UpdatedAt = DateTime.UtcNow; UpdatedBy = updatedBy; }
}
