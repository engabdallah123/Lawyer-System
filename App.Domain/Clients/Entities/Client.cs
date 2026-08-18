using App.Domain.Clients.Enums;
using App.Domain.Common;
using Shared.Domain;

namespace App.Domain.Clients.Entities;

/// <summary>
/// الموكل — الكيان الأساسي للعملاء
/// </summary>
public sealed class Client : Entity, ISoftDeletable, IAuditable
{
    public ClientType ClientType { get; private set; }
    public string? FullName { get; private set; }
    public string? CompanyName { get; private set; }
    public string? NationalId { get; private set; }
    public string? CommercialRegister { get; private set; }
    public string Phone { get; private set; } = null!;
    public string? Mobile { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }

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
    public ICollection<Cases.Entities.CaseParty> CaseParties { get; private set; } = new List<Cases.Entities.CaseParty>();
    public ICollection<PowerOfAttorney.Entities.PowerOfAttorney> PowerOfAttorneys { get; private set; } = new List<PowerOfAttorney.Entities.PowerOfAttorney>();
    public ICollection<Consultations.Entities.Consultation> Consultations { get; private set; } = new List<Consultations.Entities.Consultation>();
    public ICollection<Finance.Entities.FeeAgreement> FeeAgreements { get; private set; } = new List<Finance.Entities.FeeAgreement>();
    public ICollection<Finance.Entities.Payment> Payments { get; private set; } = new List<Finance.Entities.Payment>();
    public ICollection<Finance.Entities.Invoice> Invoices { get; private set; } = new List<Finance.Entities.Invoice>();
    public ICollection<Documents.Entities.Document> Documents { get; private set; } = new List<Documents.Entities.Document>();

    // EF Core Constructor
    private Client() { }

    private Client(
        Guid id,
        ClientType clientType,
        string? fullName,
        string? companyName,
        string? nationalId,
        string? commercialRegister,
        string phone,
        string? mobile,
        string? email,
        string? address,
        string? city,
        string? notes)
        : base(id)
    {
        ClientType = clientType;
        FullName = fullName;
        CompanyName = companyName;
        NationalId = nationalId;
        CommercialRegister = commercialRegister;
        Phone = phone;
        Mobile = mobile;
        Email = email;
        Address = address;
        City = city;
        Notes = notes;
        IsActive = true;
        IsDeleted = false;
    }

    /// <summary>
    /// إنشاء موكل جديد مع التحقق من القواعد
    /// </summary>
    public static Result<Client> Create(
        ClientType clientType,
        string? fullName,
        string? companyName,
        string? nationalId,
        string? commercialRegister,
        string phone,
        string? mobile,
        string? email,
        string? address,
        string? city,
        string? notes)
    {
        // الشخص الطبيعي يحتاج FullName
        if (clientType == ClientType.Individual && string.IsNullOrWhiteSpace(fullName))
            return Result<Client>.Failure(Errors.ClientErrors.FullNameRequired);

        // الشركة تحتاج CompanyName
        if (clientType == ClientType.Company && string.IsNullOrWhiteSpace(companyName))
            return Result<Client>.Failure(Errors.ClientErrors.CompanyNameRequired);

        // رقم الهاتف مطلوب
        if (string.IsNullOrWhiteSpace(phone))
            return Result<Client>.Failure(Errors.ClientErrors.PhoneRequired);

        var client = new Client(
            Guid.NewGuid(),
            clientType,
            fullName?.Trim(),
            companyName?.Trim(),
            nationalId?.Trim(),
            commercialRegister?.Trim(),
            phone.Trim(),
            mobile?.Trim(),
            email?.Trim(),
            address?.Trim(),
            city?.Trim(),
            notes?.Trim());

        return Result<Client>.Success(client);
    }

    /// <summary>
    /// تحديث بيانات الموكل
    /// </summary>
    public Result Update(
        ClientType clientType,
        string? fullName,
        string? companyName,
        string? nationalId,
        string? commercialRegister,
        string phone,
        string? mobile,
        string? email,
        string? address,
        string? city,
        string? notes)
    {
        if (clientType == ClientType.Individual && string.IsNullOrWhiteSpace(fullName))
            return Result.Failure(Errors.ClientErrors.FullNameRequired);

        if (clientType == ClientType.Company && string.IsNullOrWhiteSpace(companyName))
            return Result.Failure(Errors.ClientErrors.CompanyNameRequired);

        if (string.IsNullOrWhiteSpace(phone))
            return Result.Failure(Errors.ClientErrors.PhoneRequired);

        ClientType = clientType;
        FullName = fullName?.Trim();
        CompanyName = companyName?.Trim();
        NationalId = nationalId?.Trim();
        CommercialRegister = commercialRegister?.Trim();
        Phone = phone.Trim();
        Mobile = mobile?.Trim();
        Email = email?.Trim();
        Address = address?.Trim();
        City = city?.Trim();
        Notes = notes?.Trim();

        return Result.Success();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    // ISoftDeletable
    public void SoftDelete(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }

    // IAuditable
    public void SetCreated(string createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void SetUpdated(string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
