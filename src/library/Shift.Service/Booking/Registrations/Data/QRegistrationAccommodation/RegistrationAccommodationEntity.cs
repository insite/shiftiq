namespace Shift.Service.Booking;

public partial class RegistrationAccommodationEntity
{
    public Guid AccommodationIdentifier { get; set; }
    public Guid? OrganizationIdentifier { get; set; }
    public Guid RegistrationIdentifier { get; set; }

    public string? AccommodationName { get; set; }
    public string AccommodationType { get; set; } = null!;

    public int? TimeExtension { get; set; }
}