using Shift.Service.Directory;

namespace Shift.Api
{
    public class PrincipalProvider : IPrincipalProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IClaimConverter _claimConverter;

        private readonly IGroupLookupService _groupLookupService;

        private readonly PersonReader _personReader;

        private readonly QPersonSecretReader _personSecretReader;

        public PrincipalProvider(
            IHttpContextAccessor httpContextAccessor,
            IClaimConverter claimConverter,
            IGroupLookupService groupLookupService,
            PersonReader personReader,
            QPersonSecretReader personSecretReader
        )
        {
            _httpContextAccessor = httpContextAccessor;

            _claimConverter = claimConverter;

            _groupLookupService = groupLookupService;

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
            if (!await IsUserValidAsync(principal, authenticationType))
                return _claimConverter.ToPrincipal(Array.Empty<System.Security.Claims.Claim>());

            var names = await _groupLookupService.GetGroupNamesAsync(principal.RoleIds);

            foreach (var role in principal.Roles)
            {
                if (names.TryGetValue(role.Identifier, out var name))
                    role.Name = name;
            }

            return principal;
        }

        private async Task<bool> IsUserValidAsync(IPrincipal principal, string? autheticationType)
        {
            if (autheticationType != AuthenticationSchemeNames.Bearer)
                return true;

            var person = await _personReader.RetrieveAsync(principal.Person.Identifier);

            if (person == null
                || person.UserIdentifier != principal.UserId
                || person.OrganizationIdentifier != principal.OrganizationId
                || !person.IsDeveloper
            )
            {
                return false;
            }

            var personSecret = await _personSecretReader.RetrieveAsync(principal.SecretId);

            return personSecret != null
                && personSecret.PersonIdentifier == principal.Person.Identifier
                && personSecret.SecretExpiry > DateTimeOffset.UtcNow
                ;
        }

        public TimeZoneInfo GetTimeZone()
        {
            try
            {
                var id = GetPrincipal().User.TimeZone;

                var tz = Shift.Common.Base.TimeZones.GetZone(id);

                return tz;
            }
            catch
            {
                return TimeZoneInfo.Utc;
            }
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

            if (!AllowPartitionAccess(principal))
                query.OrganizationId = principal.OrganizationId;
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