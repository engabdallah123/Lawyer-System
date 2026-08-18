namespace App.Domain.Lookups;

/// <summary>
/// المحكمة — جدول مرجعي
/// </summary>
public sealed class Court
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? City { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation Properties
    public ICollection<Cases.Entities.Case> Cases { get; private set; } = new List<Cases.Entities.Case>();

    private Court() { }

    public Court(int id, string name, string? city = null)
    {
        Id = id;
        Name = name;
        City = city;
        IsActive = true;
    }

    public void Update(string name, string? city) { Name = name; City = city; }
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
