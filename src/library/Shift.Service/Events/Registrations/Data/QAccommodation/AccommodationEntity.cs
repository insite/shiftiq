namespace Shift.Service.Booking;

public partial class AccommodationEntity
{
    public RegistrationEntity? Registration { get; set; }

    public Guid AccommodationIdentifier { get; set; }
    public Guid RegistrationIdentifier { get; set; }
    public Guid? OrganizationIdentifier { get; set; }

    public string AccommodationType { get; set; } = null!;
    public string? AccommodationName { get; set; }

    public int? TimeExtension { get; set; }
}
