using System.Collections.Generic;
using System.Security.Claims;

using Shift.Common;

namespace Shift.Common
{
    public interface IClaimConverter
    {
        int CalculateLifetime(int? assigned, int? requested, int? @default);

        Claim ToClaim(ClaimName name, string value);
        IEnumerable<Claim> ToClaims(IShiftPrincipal principal);
        ClaimsIdentity ToClaimsIdentity(IJwt claims, string authenticationType);

        Dictionary<ClaimName, List<string>> ToDictionary(IEnumerable<Claim> claims);

        IShiftPrincipal ToPrincipal(IJwt jwt);
        IShiftPrincipal ToPrincipal(Dictionary<ClaimName, string> claims);
        IShiftPrincipal ToPrincipal(IEnumerable<Claim> claims);

        IShiftPrincipal ToSentinel(JwtRequest request);
    }
}