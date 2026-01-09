namespace Shift.Service.Content;

public partial class FileClaimEntity
{
    public FileEntity File { get; set; } = null!;

    public Guid ClaimIdentifier { get; set; }
    public Guid FileIdentifier { get; set; }
    public Guid ObjectIdentifier { get; set; }

    public string ObjectType { get; set; } = null!;

    public DateTimeOffset? ClaimGranted { get; set; }
}