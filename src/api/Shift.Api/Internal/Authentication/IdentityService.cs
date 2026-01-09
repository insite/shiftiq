using Shift.Service.Directory;

namespace Shift.Api
{
    public class IdentityService : IShiftIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IClaimConverter _claimConverter;

        private readonly IGroupLookupService _groupLookupService;

        public IdentityService(IHttpContextAccessor httpContextAccessor, IClaimConverter claimConverter, IGroupLookupService groupLookupService)
        {
            _httpContextAccessor = httpContextAccessor;

            _claimConverter = claimConverter;

            _groupLookupService = groupLookupService;
        }

        public Guid OrganizationId
            => GetPrincipal().Organization.Identifier;

        public Guid PersonId
            => GetPrincipal().Person?.Identifier ?? Guid.Empty;

        private const string PrincipalKey = nameof(IShiftPrincipal);

        private static MemoryCache<(Guid OrgId, Guid UserId), IShiftPrincipal> PrincipalCache = new MemoryCache<(Guid, Guid), IShiftPrincipal>();

        public IShiftPrincipal GetPrincipal()
        {
            var context = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HttpContext is not available");

            // Check context items first (fastest in-memory for request lifetime)

            if (context.Items.TryGetValue(PrincipalKey, out var cachedPrincipal)
                && cachedPrincipal is IShiftPrincipal principal)
            {
                return principal;
            }

            // Check cache if available

            var userId = GetUserIdFromClaims(context);
            var orgId = GetOrganizationIdFromClaims(context);
            var principalKey = userId == Guid.Empty || orgId == Guid.Empty
                ? (Guid.Empty, Guid.Empty)
                : (orgId, userId);

            if (PrincipalCache.TryGet(principalKey, out principal))
            {
                return principal;
            }

            // Convert from claims (most expensive operation)

            principal = context.User != null
                ? _claimConverter.ToPrincipal(context.User.Claims)
                : _claimConverter.ToPrincipal(Array.Empty<System.Security.Claims.Claim>());

            TaskRunner.RunSync(async () => await RehydrateAsync(principal));

            // Cache in both locations

            context.Items[PrincipalKey] = principal;

            PrincipalCache.Add(principalKey, principal);

            return principal;
        }

        private async Task RehydrateAsync(IShiftPrincipal principal)
        {
            var names = await _groupLookupService.GetGroupNamesAsync(principal.RoleIds);

            foreach (var role in principal.Roles)
                if (names.TryGetValue(role.Identifier, out var name))
                    role.Name = name;
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

        private Guid GetUserIdFromClaims(HttpContext context) => GetIdFromClaims(context, "user_id");

        private Guid GetOrganizationIdFromClaims(HttpContext context) => GetIdFromClaims(context, "org_id");

        private Guid GetIdFromClaims(HttpContext context, string name)
        {
            var claim = context.User != null
                ? context.User.Claims.FirstOrDefault(x => x.Type == name)
                : null;

            return claim != null && Guid.TryParse(claim.Value, out var value)
                ? value
                : Guid.Empty;
        }
    }
}