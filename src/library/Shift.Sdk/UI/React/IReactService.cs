using System.Threading.Tasks;

using Shift.Common;

namespace Shift.Contract.Presentation
{
    public interface IReactService
    {
        Task<SiteSettings> RetrieveSiteSettingsAsync(IShiftPrincipal principal, bool refresh);
        Task<PageSettings> RetrievePageSettingsAsync(IShiftPrincipal principal, string actionUrl);
    }
}