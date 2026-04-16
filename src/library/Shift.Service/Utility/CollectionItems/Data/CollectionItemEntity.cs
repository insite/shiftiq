namespace Shift.Service.Utility;

public class CollectionItemEntity
{
    public Guid CollectionIdentifier { get; set; }
    public Guid? GroupIdentifier { get; set; }
    public Guid ItemIdentifier { get; set; }
    public Guid? OrganizationIdentifier { get; set; }

    public string? ItemColor { get; set; }
    public string? ItemDescription { get; set; }
    public string? ItemFolder { get; set; }
    public string? ItemIcon { get; set; }
    public string ItemName { get; set; } = default!;
    public string? ItemNameTranslation { get; set; }

    public bool ItemIsDisabled { get; set; }

    public int ItemNumber { get; set; }
    public int ItemSequence { get; set; }

    public decimal? ItemHours { get; set; }
}
