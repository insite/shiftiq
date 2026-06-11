namespace Shift.Api;

public class VersionResponse
{
    public string Version { get; set; } = null!;

    public int Major { get; set; }

    public int Minor { get; set; }

    public int Build { get; set; }

    public int Revision { get; set; }
}