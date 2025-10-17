using Shift.Common;

namespace Shift.Api
{
    public class IdentityService : IShiftIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IClaimConverter _claimConverter;

        public IdentityService(IHttpContextAccessor httpContextAccessor, IClaimConverter claimConverter)
        {
            _httpContextAccessor = httpContextAccessor;

            _claimConverter = claimConverter;
        }

        public Guid OrganizationId
            => GetPrincipal().Organization.Identifier;

        public Guid PersonId
            => GetPrincipal().Person?.Identifier ?? Guid.Empty;

        private const string PrincipalKey = nameof(IShiftPrincipal);

        private static MemoryCache<string, IShiftPrincipal> PrincipalCache = new MemoryCache<string, IShiftPrincipal>();

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

            if (PrincipalCache.TryGet(userId, out principal))
            {
                return principal;
            }

            // Convert from claims (most expensive operation)

            principal = context.User != null
                ? _claimConverter.ToPrincipal(context.User.Claims)
                : _claimConverter.ToPrincipal(Array.Empty<System.Security.Claims.Claim>());

            // Cache in both locations

            context.Items[PrincipalKey] = principal;

            PrincipalCache.Add(userId, principal);

            return principal;
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

        private string GetUserIdFromClaims(HttpContext context)
        {
            if (context.User != null)
            {
                var claim = context.User.Claims.FirstOrDefault(x => x.Type == "user_id");

                if (claim != null)
                    return claim.Value;
            }

            return Guid.Empty.ToString();
        }
    }
}
