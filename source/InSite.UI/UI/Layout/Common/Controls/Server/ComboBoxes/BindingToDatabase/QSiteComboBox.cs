using InSite.Application.Sites.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class QSiteComboBox : ComboBox
    {
        public QSiteFilter ListFilter => (QSiteFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new QSiteFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier }));

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            ListFilter.OrderBy = "SiteDomain";
            ListFilter.Paging = null;

            var data = ServiceLocator.SiteSearch.Bind(x => x, ListFilter);
            foreach (var item in data)
                list.Add(item.SiteIdentifier.ToString(), item.SiteDomain);

            return list;
        }
    }
}