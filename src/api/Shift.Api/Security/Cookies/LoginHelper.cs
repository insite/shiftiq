using Shift.Service.Directory;

namespace Shift.Api;

public class LoginHelper(
    CookieSettings cookieSettings,
    PersonService personService,
    OrganizationService organizationService,
    GroupService groupService
    )
{
    public async Task<string?> LoginAsync(string organizationCode, string email)
    {
        var orgs = await organizationService.SearchAsync(new SearchOrganizations
        {
            OrganizationCode = organizationCode
        }, default);

        var organization = orgs.FirstOrDefault();
        if (organization == null)
            return null;

        var organizationId = organization.OrganizationIdentifier;
        var parentOrganizationId = organization.ParentOrganizationIdentifier;

        var people = await personService.SearchAsync(new SearchPeople
        {
            OrganizationIdentifier = organizationId,
            EmailExact = email
        });

        var person = people.FirstOrDefault();
        if (person == null)
            return null;

        var userRoles = await groupService.SearchUserRolesAsync(parentOrganizationId, organizationId, person.UserId);

        var token = CreateToken(organizationCode, organizationId, person, userRoles);

        var encrypt = cookieSettings.Encrypt;
        var secret = cookieSettings.Secret;

        return new CookieTokenEncoder().Serialize(token, encrypt, secret);
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