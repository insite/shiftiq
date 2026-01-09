using Shift.Service.Security;

namespace Shift.Service.Content;

public partial class FileEntity
{
    public ICollection<FileActivityEntity> Activities { get; set; } = new List<FileActivityEntity>();
    public ICollection<FileClaimEntity> Claims { get; set; } = new List<FileClaimEntity>();

    public OrganizationEntity Organization { get; set; } = null!;
    public UserEntity User { get; set; } = null!;

    public Guid? ApprovedUserIdentifier { get; set; }
    public Guid FileIdentifier { get; set; }
    public Guid? LastActivityUserIdentifier { get; set; }
    public Guid ObjectIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid? ReviewedUserIdentifier { get; set; }
    public Guid UserIdentifier { get; set; }

    public string DocumentName { get; set; } = null!;
    public string? FileCategory { get; set; }
    public string FileContentType { get; set; } = null!;
    public string? FileDescription { get; set; }
    public string FileLocation { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string? FilePath { get; set; }
    public string FileStatus { get; set; } = null!;
    public string? FileSubcategory { get; set; }
    public string? FileUrl { get; set; }
    public string ObjectType { get; set; } = null!;

    public int FileSize { get; set; }

    public bool AllowLearnerToView { get; set; }

    public DateTimeOffset? ApprovedTime { get; set; }
    public DateTimeOffset? FileAlternated { get; set; }
    public DateTimeOffset? FileExpiry { get; set; }
    public DateTimeOffset? FileReceived { get; set; }
    public DateTimeOffset FileUploaded { get; set; }
    public DateTimeOffset? LastActivityTime { get; set; }
    public DateTimeOffset? ReviewedTime { get; set; }
}