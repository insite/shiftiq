using System.Collections.Generic;
using System.Security.Claims;

namespace Shift.Common
{
    public interface IClaimConverter
    {
        int CalculateLifetime(int? assigned, int? requested, int? @default);

        Claim ToClaim(ClaimName name, string value);
        IEnumerable<Claim> ToClaims(IPrincipal principal);
        ClaimsIdentity ToClaimsIdentity(IJwt claims, string authenticationType);

        Dictionary<ClaimName, List<string>> ToDictionary(IEnumerable<Claim> claims);

        IPrincipal ToPrincipal(IJwt jwt);
        IPrincipal ToPrincipal(Dictionary<ClaimName, string> claims);
        IPrincipal ToPrincipal(IEnumerable<Claim> claims);

        bool IsSentinel(string sentinelSecret);
        IPrincipal ToSentinel(JwtRequest sentinelRequest);
    }
}