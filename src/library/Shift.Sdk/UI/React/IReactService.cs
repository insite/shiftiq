using System;
using System.Threading.Tasks;

using Shift.Common;

namespace Shift.Contract.Presentation
{
    public interface IReactService
    {
        Task<SiteSettings> RetrieveSiteSettingsAsync(IPrincipal principal, bool refresh);
        Task<PageSettings> RetrievePageSettingsAsync(IPrincipal principal, string actionUrl);
    }
}