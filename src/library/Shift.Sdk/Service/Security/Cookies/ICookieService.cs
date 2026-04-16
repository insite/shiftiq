using System.Threading.Tasks;

using Shift.Contract;

namespace Shift.Sdk.Service.Security.Cookies
{
    public interface ICookieService
    {
        bool Debug { get; }
        string Path { get; }

        string GetSecurityDomain();
        CookieToken GetCookieToken();
        void AppendSecurityCookie(CookieToken token);
        void DeleteSecurityCookie();
    }
}
