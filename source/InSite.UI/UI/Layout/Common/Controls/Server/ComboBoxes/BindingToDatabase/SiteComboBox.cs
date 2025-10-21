using System.Linq;

using InSite.Application.Sites.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class SiteComboBox : ComboBox
    {
        public override bool AllowBlank => false;

        public QSiteFilter ListFilter => (QSiteFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new QSiteFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier
            }));

        protected override ListItemArray CreateDataSource()
        {
            ListFilter.Domain = null;
            ListFilter.OrderBy = "SiteDomain";

            var list = new ListItemArray
            {
                TotalCount = ServiceLocator.SiteSearch.Count(ListFilter)
            };

            var data = ServiceLocator.SiteSearch
                .Bind(x => new
                {
                    x.SiteIdentifier,
                    x.SiteTitle,
                    x.SiteDomain,
                    PageCount = x.Pages.Count()
                }, ListFilter)
                .ToList();

            list.Add(new ListItem());

            foreach (var item in data)
                list.Add(item.SiteIdentifier.ToString(), item.SiteTitle);

            return list;
        }
    }
}