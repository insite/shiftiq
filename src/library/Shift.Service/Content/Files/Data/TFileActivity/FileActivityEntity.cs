namespace Shift.Service.Content;

public partial class FileActivityEntity
{
    public FileEntity File { get; set; } = null!;

    public Guid ActivityIdentifier { get; set; }
    public Guid FileIdentifier { get; set; }
    public Guid UserIdentifier { get; set; }

    public string ActivityChanges { get; set; } = null!;

    public DateTimeOffset ActivityTime { get; set; }
}