using System.Collections.Generic;
using System.Security.Claims;

namespace Shift.Common
{
    public interface IPrincipalSearch
    {
        IPrincipal GetPrincipal(string secret);

        IPrincipal GetPrincipal(JwtRequest request, string ipAddress, bool isWhitelisted, int? lifetime, List<string> errors);

        string GetSecret(IEnumerable<Claim> claims);
        string GetSecret(IPrincipal principal);
    }
}