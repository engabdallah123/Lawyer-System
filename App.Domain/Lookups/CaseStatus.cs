namespace App.Domain.Lookups;

/// <summary>
/// حالة القضية — جدول مرجعي
/// </summary>
public sealed class CaseStatus
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Color { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation Properties
    public ICollection<Cases.Entities.Case> Cases { get; private set; } = new List<Cases.Entities.Case>();

    private CaseStatus() { }

    public CaseStatus(string name, string? color = null)
    {
        Name = name;
        Color = color;
        IsActive = true;
    }

    public CaseStatus(int id, string name, string? color = null)
    {
        Id = id;
        Name = name;
        Color = color;
        IsActive = true;
    }

    public void Update(string name, string? color) { Name = name; Color = color; }
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
