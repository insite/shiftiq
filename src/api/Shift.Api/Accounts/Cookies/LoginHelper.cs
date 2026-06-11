using Shift.Service.Directory;

namespace Shift.Api;

public class LoginHelper(
    PersonService personService,
    OrganizationService organizationService,
    GroupService groupService,
    IPrincipalProvider identityService
    )
{
    public async Task<CookieToken?> LoginAsync(string organizationCode, string email, string? impersonatorOrganizationCode, string? impersonatorUserEmail)
    {
        var (organizationId, person) = await FindPersonAsync(organizationCode, email);
        if (organizationId == null || person == null)
            return null;

        if (!string.IsNullOrEmpty(impersonatorOrganizationCode) && !string.IsNullOrEmpty(impersonatorUserEmail))
        {
            var (impersonatorOrganizationId, impersonatorPerson) = await FindPersonAsync(impersonatorOrganizationCode, impersonatorUserEmail);
            if (impersonatorOrganizationId == null || impersonatorPerson == null)
            {
                impersonatorOrganizationCode = null;
                impersonatorUserEmail = null;
            }
        }
        else
        {
            impersonatorOrganizationCode = null;
            impersonatorUserEmail = null;
        }

        var principal = identityService.GetPrincipal();

        var userRoles = await groupService.SearchUserRolesAsync(principal.Partition.Identifier, organizationId.Value, person.UserId);

        return CreateToken(organizationCode, organizationId.Value, person, userRoles, impersonatorOrganizationCode, impersonatorUserEmail);
    }

    private async Task<(Guid? organizationId, PersonMatch? person)> FindPersonAsync(string organizationCode, string email)
    {
        var orgs = await organizationService.SearchAsync(new SearchOrganizations
        {
            OrganizationCode = organizationCode
        }, default);

        var organization = orgs.FirstOrDefault();
        if (organization == null)
            return (null, null);

        var organizationId = organization.OrganizationId;

        var people = await personService.SearchAsync(new SearchPeople
        {
            OrganizationId = organizationId,
            EmailExact = email
        });

        var person = people.FirstOrDefault();
        if (person == null)
            return (null, null);

        return (organizationId, person);
    }

    private static CookieToken CreateToken(
        string organizationCode,
        Guid organizationId,
        PersonMatch person,
        string[] userRoles,
        string? impersonatorOrganizationCode,
        string? impersonatorUserEmail
    )
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
            ImpersonatorOrganization = impersonatorOrganizationCode,
            ImpersonatorUser = impersonatorUserEmail,
            TimeZoneId = person.TimeZone,
            AuthenticationSource = "Test Login"
        };
    }
}