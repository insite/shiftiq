using Shift.Service.Content;
using Shift.Service.Directory;

namespace Shift.Service.Security;

public partial class OrganizationEntity
{
    public ICollection<PersonEntity> People { get; set; } = new List<PersonEntity>();
    public ICollection<TPermissionEntity> Permissions { get; set; } = new List<TPermissionEntity>();
    public ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();

    public Guid? AdministratorUserIdentifier { get; set; }
    public Guid? GlossaryIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid? ParentOrganizationIdentifier { get; set; }

    public string AccountStatus { get; set; } = null!;
    public string? CompanyDomain { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanySize { get; set; }
    public string? CompanySummary { get; set; }
    public string? CompanyTitle { get; set; }
    public string? CompanyWebSiteUrl { get; set; }
    public string CompetencyAutoExpirationMode { get; set; } = null!;
    public string? OrganizationCode { get; set; }
    public string OrganizationData { get; set; } = null!;
    public string? OrganizationLogoUrl { get; set; }
    public string? PersonFullNamePolicy { get; set; }
    public string? StandardContentLabels { get; set; }
    public string? TimeZone { get; set; }

    public int? CompetencyAutoExpirationDay { get; set; }
    public int? CompetencyAutoExpirationMonth { get; set; }

    public DateTimeOffset? AccountClosed { get; set; }
    public DateTimeOffset? AccountOpened { get; set; }
}