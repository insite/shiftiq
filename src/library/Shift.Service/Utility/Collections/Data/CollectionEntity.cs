namespace Shift.Service.Utility;

public class CollectionEntity
{
    public Guid CollectionIdentifier { get; set; }

    public string CollectionName { get; set; } = default!;
    public string? CollectionPackage { get; set; }
    public string CollectionProcess { get; set; } = default!;
    public string? CollectionReferences { get; set; }
    public string CollectionTool { get; set; } = default!;
    public string CollectionType { get; set; } = default!;
}
