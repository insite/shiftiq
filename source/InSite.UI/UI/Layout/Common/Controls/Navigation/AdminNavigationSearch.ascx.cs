using System.Web.UI;

namespace InSite.UI.Layout.Common.Controls.Navigation
{
    public partial class AdminNavigationSearch : UserControl
    {
        protected string GetAdminNavigationSearchUrl()
        {
            return ServiceLocator.AppSettings.v2ApiBaseUrl.TrimEnd('/') + "/api/assets/searches";
        }
    }
}
