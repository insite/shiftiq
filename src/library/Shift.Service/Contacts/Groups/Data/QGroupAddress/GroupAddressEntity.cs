namespace Shift.Service.Directory;

public class GroupAddressEntity
{
    public GroupEntity? Group { get; set; }

    public Guid AddressIdentifier { get; set; }

    public Guid GroupIdentifier { get; set; }
    public string AddressType { get; set; } = null!;
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Description { get; set; }
    public string? PostalCode { get; set; }
    public string? Province { get; set; }
    public string? Street1 { get; set; }
    public string? Street2 { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public Guid? OrganizationIdentifier { get; set; }
}
