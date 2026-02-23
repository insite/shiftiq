using Shift.Service.Directory;

namespace Shift.Api;

public class LoginHelper(
    PersonService personService,
    OrganizationService organizationService,
    GroupService groupService,
    IPrincipalProvider identityService
    )
{
    public async Task<CookieToken?> LoginAsync(string organizationCode, string email)
    {
        var orgs = await organizationService.SearchAsync(new SearchOrganizations
        {
            OrganizationCode = organizationCode
        }, default);

        var organization = orgs.FirstOrDefault();
        if (organization == null)
            return null;

        var organizationId = organization.OrganizationId;

        var people = await personService.SearchAsync(new SearchPeople
        {
            OrganizationId = organizationId,
            EmailExact = email
        });

        var person = people.FirstOrDefault();
        if (person == null)
            return null;

        var principal = identityService.GetPrincipal();

        var userRoles = await groupService.SearchUserRolesAsync(principal.Partition.Identifier, organizationId, person.UserId);

        return CreateToken(organizationCode, organizationId, person, userRoles);
    }

    private static CookieToken CreateToken(string organizationCode, Guid organizationId, PersonMatch person, string[] userRoles)
    {
        return new CookieToken
        {
            Language = "en",
            Environment = "Local",
            OrganizationCode = organizationCode,
            OrganizationIdentifier = organizationId,
            UserEmail = person.UserEmail,
            UserIdentifier = person.UserId,
            IsAdministrator = person.IsAdministrator,
            IsDeveloper = person.IsDeveloper,
            IsOperator = person.IsOperator,
            UserRoles = userRoles,
            ImpersonatorOrganization = null,
            ImpersonatorUser = null,
            TimeZoneId = person.TimeZone,
            AuthenticationSource = "Test Login"
        };
    }
}