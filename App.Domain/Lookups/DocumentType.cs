namespace App.Domain.Lookups;

/// <summary>
/// نوع المستند — جدول مرجعي (صحيفة دعوى، مذكرة دفاع، تقرير خبير، حكم، عقد، إلخ)
/// </summary>
public sealed class DocumentType
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation Properties
    public ICollection<Documents.Entities.Document> Documents { get; private set; } = new List<Documents.Entities.Document>();

    private DocumentType() { }

    public DocumentType(int id, string name, string? description = null)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = true;
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
