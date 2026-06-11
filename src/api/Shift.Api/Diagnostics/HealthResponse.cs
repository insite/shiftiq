namespace Shift.Api;

public class HealthResponse
{
    public string? Status { get; set; }

    public string? Version { get; set; }

    public object? Environment { get; set; }

    public HealthConfiguration? Configuration { get; set; }
}
