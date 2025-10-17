namespace Shift.Service.Site;

public partial class QSiteEntity
{
    public ICollection<QPageEntity> Pages { get; set; } = new List<QPageEntity>();

    public Guid OrganizationIdentifier { get; set; }
    public Guid SiteIdentifier { get; set; }

    public string? LastChangeType { get; set; }
    public string? LastChangeUser { get; set; }
    public string SiteDomain { get; set; } = null!;
    public string SiteTitle { get; set; } = null!;

    public DateTimeOffset? LastChangeTime { get; set; }
}