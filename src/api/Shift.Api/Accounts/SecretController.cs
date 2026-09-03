using Microsoft.AspNetCore.Mvc;

using Shift.Constant;
using Shift.Service.Directory;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Accounts API")]
public class SecretController : ShiftControllerBase
{
    private readonly IPrincipalProvider _identityService;
    private readonly PersonService _personService;
    private readonly PersonSecretService _personSecretService;
    private readonly SecuritySettings _securitySettings;

    public SecretController(
        IPrincipalProvider identityService,
        PersonService personService,
        PersonSecretService personSecretService,
        SecuritySettings securitySettings)
    {
        _identityService = identityService;
        _personService = personService;
        _personSecretService = personSecretService;
        _securitySettings = securitySettings;
    }

    /// <summary>
    /// Generates a new client secret for the authenticated user, replacing any existing secret.
    /// </summary>
    /// <param name="expiry">
    /// The number of days until the secret expires.
    /// - Minimum: 1 Day
    /// - Maximum: 365 Days
    /// - Default (if omitted): 90 Days
    /// </param>
    [HttpPost("api/accounts/secrets/generate")]
    [EndpointName("generateSecret")]
    [HybridAuthorize]
    [ProducesResponseType(typeof(PersonSecretModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonSecretModel>> GenerateAsync(int? expiry = null)
    {
        var principal = _identityService.GetPrincipal();

        if (principal.UserId == Guid.Empty)
            return BadRequest("User");

        if (principal.OrganizationId == Guid.Empty)
            return BadRequest("Organization");

        var people = await _personService.SearchAsync(new SearchPeople
        {
            OrganizationId = principal.OrganizationId,
            UserId = principal.UserId
        });

        var person = people.FirstOrDefault();

        if (person == null)
            return BadRequest("person");

        var personId = person.PersonId;

        var tokenSettings = _securitySettings.Token;

        var tokenLifetimeInMinutes = tokenSettings.Lifetime;

        var secretExpiryInDays = expiry ?? tokenSettings.GetClientSecretLifetimeInDays();

        var secret = await GetClientSecretAsync(personId, secretExpiryInDays, tokenLifetimeInMinutes);

        return Ok(secret);
    }

    [HttpGet("api/accounts/secrets/introspect")]
    [EndpointName("introspectSecret")]
    [SecretAuthorize]
    public IActionResult IntrospectAsync()
    {
        var principal = _identityService.GetPrincipal();

        return Ok(principal);
    }

    private async Task<PersonSecretModel> GetClientSecretAsync(Guid personId, int secretExpiryInDays, int tokenLifetimeInMinutes)
    {
        var name = SecretName.ShiftClientSecret;

        var type = SecretType.Authentication;

        var existing = await _personSecretService.CollectAsync(new CollectPersonSecrets
        {
            PersonId = personId,
            SecretName = name,
            Filter = new QueryFilter()
        });

        foreach (var item in existing)
            await _personSecretService.DeleteAsync(item.SecretId);

        var secretId = UniqueIdentifier.Create();

        var value = Secret.CreateValue();

        secretExpiryInDays = ValidateLifetimeInDays(secretExpiryInDays);

        var secretExpiry = DateTimeOffset.Now.AddDays(secretExpiryInDays);

        await _personSecretService.CreateAsync(new CreatePersonSecret
        {
            PersonId = personId,
            SecretId = secretId,
            SecretType = type,
            SecretName = name,
            SecretValue = value,
            SecretLifetimeLimit = ValidateLifetimeInMinutes(tokenLifetimeInMinutes),
            SecretExpiry = secretExpiry
        });

        var secret = await _personSecretService.RetrieveAsync(secretId);

        if (secret != null)
            return secret;

        throw new InvalidOperationException($"Unable retrieve new secret {secretId}");
    }

    private int ValidateLifetimeInMinutes(int minutes)
    {
        var minimum = 1;
        var maximum = 365 * 24 * 60; // 365 days x 24 hours x 60 minutes = 525,600 minutes

        if (minutes < minimum || maximum < minutes)
            return maximum;

        return minutes;
    }

    private int ValidateLifetimeInDays(int days)
    {
        var minimum = 1;
        var maximum = 365; // 365 days

        if (days < minimum || maximum < days)
            return maximum;

        return days;
    }
}
