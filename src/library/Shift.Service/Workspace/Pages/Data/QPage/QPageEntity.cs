namespace Shift.Service.Site;

public partial class QPageEntity
{
    public QSiteEntity? Site { get; set; }
    public QPageEntity? Parent { get; set; }
    public ICollection<QPageEntity> Children { get; set; } = new List<QPageEntity>();

    public Guid? ObjectIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid PageIdentifier { get; set; }
    public Guid? ParentPageIdentifier { get; set; }
    public Guid? SiteIdentifier { get; set; }

    public bool IsAccessDenied { get; set; }
    public bool IsHidden { get; set; }
    public bool IsNewTab { get; set; }

    public string? AuthorName { get; set; }
    public string? ContentControl { get; set; }
    public string? ContentLabels { get; set; }
    public string? Hook { get; set; }
    public string? LastChangeType { get; set; }
    public string? LastChangeUser { get; set; }
    public string? NavigateUrl { get; set; }
    public string? ObjectType { get; set; }
    public string? PageIcon { get; set; }
    public string? PageSlug { get; set; }
    public string PageType { get; set; } = null!;
    public string Title { get; set; } = null!;

    public int Sequence { get; set; }

    public DateTimeOffset? AuthorDate { get; set; }
    public DateTimeOffset? LastChangeTime { get; set; }
}