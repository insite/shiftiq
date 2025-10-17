using System.Collections.Generic;
using System.Security.Claims;

using Shift.Common;

namespace Shift.Common
{
    public interface IPrincipalSearch
    {
        IShiftPrincipal GetPrincipal(string secret);

        IShiftPrincipal GetPrincipal(JwtRequest request, string ipAddress, bool isWhitelisted, int? lifetime, List<string> errors);

        string GetSecret(IEnumerable<Claim> claims);
    }
}