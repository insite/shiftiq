namespace Shift.Api;

public interface IPrincipalProvider
{
    IPrincipal GetPrincipal();

    Guid OrganizationId { get; }

    TimeZoneInfo GetTimeZone();

    void ValidateOrganizationId(IPrincipal principal, IQueryByOrganization query);

    Guid? GetOrganizationId(IPrincipal principal);

    bool AllowOrganizationAccess(IPrincipal principal, Guid organization);
}
