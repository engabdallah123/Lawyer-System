namespace App.Application.Lookups.DTOs;

public class LookupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Extra { get; set; }

    public LookupDto() { }

    public LookupDto(int id, string name, string? description = null, string? extra = null)
    {
        Id = id;
        Name = name;
        Description = description;
        Extra = extra;
    }
}
