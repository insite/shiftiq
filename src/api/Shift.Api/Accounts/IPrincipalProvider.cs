namespace Shift.Api;

public interface IPrincipalProvider
{
    IPrincipal GetPrincipal();

    Guid OrganizationId { get; }

    void ValidateOrganizationId(IPrincipal principal, IQueryByOrganization query);

    Guid? GetOrganizationId(IPrincipal principal);

    bool AllowOrganizationAccess(IPrincipal principal, Guid organization);
}
