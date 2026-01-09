using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.Admin.Utilities.Actions.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TActionFilter>
    {
        protected override IListSource SelectData(TActionFilter filter)
        {
            var items = TActionSearch.SearchResult(filter);

            foreach (var item in items)
            {
                var permissions = TGroupPermissionSearch.Bind(x => x.GroupIdentifier, x => x.ObjectIdentifier == item.ActionIdentifier);

                item.GroupCount = permissions.Count;
            }

            return items.ToSearchResult();
        }

        protected override int SelectCount(TActionFilter filter)
        {
            return TActionSearch.Count(filter);
        }

        protected string GetActionIconHtml(object o)
        {
            return o is string icon ? $"<i class='{icon}'/></i>" : string.Empty;
        }

        protected string GetActionListHtml(object o)
        {
            return o is string list ? $"<span class='badge bg-primary fs-sm'>{list}</span>" : string.Empty;
        }

        protected string GetAuthorizationRequirementHtml(object o)
        {
            return o is string authorization ? $"<span class='badge bg-success fs-sm'>{authorization}</span>" : string.Empty;
        }

        protected string GetExtraBreadcrumbHtml(object o)
        {
            return o is string breadcrumb ? $"<span class='badge bg-info fs-sm'>{breadcrumb}</span>" : string.Empty;
        }

        protected string GetHelpHtml(object o)
        {
            var item = (TActionSearchItem)o;

            var html = "<div>";

            if (item.HelpUrl != null)
            {
                html += "<span class='badge bg-success'>Action Linked to Help</span>";

                var helpUrl = ServiceLocator.AppSettings.Application.HelpUrl + item.HelpUrl;

                var helpLink = $"<a title='View help topic' target='_blank' href='{helpUrl}'>{item.HelpUrl}</a>";

                html += $"<span class='text-danger ms-2'>{item.HelpUrl}</span>";
            }

            html += "</div>";

            return html;
        }
    }
}