using Shared.Domain;

namespace App.Domain.Documents.Entities;

/// <summary>
/// نسخة المستند — الملف الفعلي مع Versioning
/// </summary>
public sealed class DocumentVersion : Entity
{
    public Guid DocumentId { get; private set; }
    public int VersionNumber { get; private set; }
    public string FilePath { get; private set; } = null!;
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long FileSize { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public string UploadedBy { get; private set; } = null!;
    public string? Notes { get; private set; }

    // Navigation Properties
    public Document Document { get; private set; } = null!;

    private DocumentVersion() { }

    private DocumentVersion(
        Guid id, Guid documentId, int versionNumber, string filePath,
        string fileName, string contentType, long fileSize, string uploadedBy, string? notes)
        : base(id)
    {
        DocumentId = documentId;
        VersionNumber = versionNumber;
        FilePath = filePath;
        FileName = fileName;
        ContentType = contentType;
        FileSize = fileSize;
        UploadedAt = DateTime.UtcNow;
        UploadedBy = uploadedBy;
        Notes = notes;
    }

    public static Result<DocumentVersion> Create(
        Guid documentId, int versionNumber, string filePath,
        string fileName, string contentType, long fileSize, string uploadedBy, string? notes)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Result<DocumentVersion>.Failure(Errors.DocumentErrors.FilePathRequired);

        if (string.IsNullOrWhiteSpace(fileName))
            return Result<DocumentVersion>.Failure(Errors.DocumentErrors.FileNameRequired);

        var version = new DocumentVersion(
            Guid.NewGuid(), documentId, versionNumber, filePath,
            fileName, contentType, fileSize, uploadedBy, notes?.Trim());

        return Result<DocumentVersion>.Success(version);
    }
}
