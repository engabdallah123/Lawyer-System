namespace App.Domain.Lookups;

/// <summary>
/// نوع القضية — جدول مرجعي
/// </summary>
public sealed class CaseType
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation Properties
    public ICollection<Cases.Entities.Case> Cases { get; private set; } = new List<Cases.Entities.Case>();

    private CaseType() { }

    public CaseType(int id, string name, string? description = null)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = true;
    }

    public void Update(string name, string? description) { Name = name; Description = description; }
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
