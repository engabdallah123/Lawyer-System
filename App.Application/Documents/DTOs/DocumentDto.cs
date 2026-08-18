namespace App.Application.Documents.DTOs;

public record DocumentVersionDto(
    Guid Id,
    Guid DocumentId,
    int VersionNumber,
    string FilePath,
    string FileName,
    string ContentType,
    long FileSize,
    DateTime UploadedAt,
    string UploadedBy,
    string? Notes);

public record DocumentDto(
    Guid Id,
    Guid? CaseId,
    string? CaseInternalNumber,
    string? CaseTitle,
    Guid? ClientId,
    string? ClientName,
    int? DocumentTypeId,
    string? DocumentTypeName,
    string Name,
    string? Description,
    Guid? CurrentVersionId,
    int CurrentVersionNumber,
    string? CurrentFilePath,
    string? CurrentFileName,
    long? CurrentFileSize,
    DateTime CreatedAt,
    string CreatedBy);

public record DocumentDetailsDto(
    Guid Id,
    Guid? CaseId,
    string? CaseInternalNumber,
    string? CaseTitle,
    Guid? ClientId,
    string? ClientName,
    int? DocumentTypeId,
    string? DocumentTypeName,
    string Name,
    string? Description,
    Guid? CurrentVersionId,
    DateTime CreatedAt,
    string CreatedBy,
    IEnumerable<DocumentVersionDto> Versions);
