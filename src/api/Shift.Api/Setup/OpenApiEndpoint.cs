namespace Shift.Api;

public class OpenApiEndpoint
{
    public string Route { get; set; } = null!;
    public string Method { get; set; } = null!;
    public string? Summary { get; set; }
    public string? Description { get; set; }
    public string? OperationId { get; set; }
    public bool RequiresAuthorization { get; set; }
    public List<string> AuthorizationPolicies { get; set; } = new List<string>();
    public List<string> SecuritySchemes { get; set; } = new List<string>();
}
