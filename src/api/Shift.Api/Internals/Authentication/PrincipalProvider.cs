using Shift.Service.Directory;

namespace Shift.Api
{
    public class PrincipalProvider : IPrincipalProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IClaimConverter _claimConverter;

        private readonly GroupService _groupService;

        private readonly PersonReader _personReader;

        private readonly QPersonSecretReader _personSecretReader;

        public PrincipalProvider(
            IHttpContextAccessor httpContextAccessor,
            IClaimConverter claimConverter,
            GroupService groupService,
            PersonReader personReader,
            QPersonSecretReader personSecretReader
        )
        {
            _httpContextAccessor = httpContextAccessor;

            _claimConverter = claimConverter;

            _groupService = groupService;

            _personReader = personReader;

            _personSecretReader = personSecretReader;
        }

        public Guid OrganizationId
            => GetPrincipal().Organization.Identifier;

        public Guid PersonId
            => GetPrincipal().Person?.Identifier ?? Guid.Empty;

        private const string PrincipalKey = nameof(IPrincipal);

        private static MemoryCache<(Guid OrgId, Guid UserId), (Guid CookieId, IPrincipal Principal)> PrincipalCache = new();

        public IPrincipal GetPrincipal()
        {
            var context = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HttpContext is not available");

            // Check context items first (fastest in-memory for request lifetime)

            if (context.Items.TryGetValue(PrincipalKey, out var cachedPrincipal)
                && cachedPrincipal is IPrincipal principal)
            {
                return principal;
            }

            // Check cache if available

            var userId = GetUserIdFromClaims(context);
            var orgId = GetOrganizationIdFromClaims(context);
            var cookieId = GetCookieIdFromClaims(context);

            var principalKey = userId == Guid.Empty || orgId == Guid.Empty
                ? (Guid.Empty, Guid.Empty)
                : (orgId, userId);

            if (cookieId != Guid.Empty && orgId != Guid.Empty && userId != Guid.Empty)
            {
                if (PrincipalCache.TryGet(principalKey, out var cachedValue) && cachedValue.CookieId == cookieId)
                {
                    return cachedValue.Principal;
                }
            }

            // Convert from claims (most expensive operation)

            if (context.User != null)
            {
                principal = _claimConverter.ToPrincipal(context.User.Claims);
                
                TaskRunner.RunSync(async () => principal = await RehydrateAsync(principal, context.User.Identity?.AuthenticationType));
            }
            else
                principal = _claimConverter.ToPrincipal(Array.Empty<System.Security.Claims.Claim>());

            // Cache in both locations

            context.Items[PrincipalKey] = principal;

            PrincipalCache.Add(principalKey, (cookieId, principal));

            return principal;
        }

        private async Task<IPrincipal> RehydrateAsync(IPrincipal principal, string? authenticationType)
        {
            var person = await ValidatePersonAsync(principal, authenticationType);
            if (person == null)
                return _claimConverter.ToPrincipal(Array.Empty<System.Security.Claims.Claim>());

            principal.Roles.Clear();

            var roles = await _groupService.SearchUserRolesAsync(principal.Partition.Identifier, principal.OrganizationId, principal.UserId);
            foreach (var role in roles)
                principal.Roles.Add(new Role(role.GroupName, role.GroupId));

            var (authority, systemRoles) = PrincipalSearch.GetAuthority(person);

            principal.Authority = authority;
            principal.Roles.AddRange(systemRoles);

            return principal;
        }

        private async Task<PersonMatch?> ValidatePersonAsync(IPrincipal principal, string? authenticationType)
        {
            if (principal.UserId == Guid.Empty || principal.OrganizationId == Guid.Empty)
                return null;

            var people = await _personReader.SearchAsync(new SearchPeople
            {
                UserId = principal.UserId,
                OrganizationId = principal.OrganizationId
            });

            if (people.Length != 1)
                return null;

            var person = people[0];

            if (authenticationType != AuthenticationSchemeNames.Bearer)
                return person;

            if (!person.IsDeveloper)
                return null;

            var personSecret = await _personSecretReader.RetrieveAsync(principal.SecretId);

            return personSecret != null
                    && personSecret.PersonIdentifier == principal.Person.Identifier
                    && personSecret.SecretExpiry > DateTimeOffset.UtcNow
                ? person
                : null;
        }

        private static Guid GetUserIdFromClaims(HttpContext context) => GetIdFromClaims(context, ClaimName.UserId);

        private static Guid GetOrganizationIdFromClaims(HttpContext context) => GetIdFromClaims(context, ClaimName.OrganizationId);

        private static Guid GetCookieIdFromClaims(HttpContext context) => GetIdFromClaims(context, ClaimName.CookieId);

        private static Guid GetIdFromClaims(HttpContext context, ClaimName name)
        {
            var claim = context.User != null
                ? context.User.Claims.FirstOrDefault(x => x.Type == name.ToJwtName())
                : null;

            return claim != null && Guid.TryParse(claim.Value, out var value)
                ? value
                : Guid.Empty;
        }

        public void ValidateOrganizationId(IPrincipal principal, IQueryByOrganization query)
        {
            // If the caller is not permitted to query for data owned by other organizations, then force the principal's
            // organization identifier into the query.

            if (AllowPartitionAccess(principal))
                return;

            query.OrganizationId = principal.OrganizationId;

            if (query is IQuery q && q.Filter != null && q.Filter.PageSize > QueryFilter.MaxPageSize)
                q.Filter.PageSize = QueryFilter.MaxPageSize;
        }

        private bool AllowPartitionAccess(IPrincipal principal)
        {
            return principal.IsOperator;
        }

        public bool AllowOrganizationAccess(IPrincipal principal, Guid organizationId)
        {
            return principal.IsOperator || principal.OrganizationId == organizationId;
        }

        public Guid? GetOrganizationId(IPrincipal principal)
        {
            return principal.OrganizationId;
        }
    }
}
