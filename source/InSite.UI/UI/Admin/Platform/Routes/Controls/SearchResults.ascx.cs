using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

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

                if (item.HelpUrl.IsNotEmpty())
                {
                    item.HelpTopicIdentifier = HelpTopics.FindTopicByUrl(item.HelpUrl)?.Identifier;
                }
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

        protected string GetHelpHtml(object o)
        {
            var item = (TActionSearchItem)o;

            var html = "<div>";

            if (item.HelpUrl != null)
            {
                html += "<span class='badge bg-success'>Action Linked to Help</span>";

                var helpUrl = ServiceLocator.AppSettings.Application.HelpUrl + item.HelpUrl;
                var helpLink = $"<a title='View help topic' target='_blank' href='{helpUrl}'>{item.HelpUrl}</a>";

                if (item.HelpTopicIdentifier.HasValue)
                {
                    html += $"<span class='text-success ms-2'>{helpLink}<a title='Edit help topic' target='_blank' href='/ui/admin/sites/pages/outline?panel=content&id={item.HelpTopicIdentifier.Value}' class='ms-2'><i class='fas fa-pencil'></i></a></span>";
                }
                else
                {
                    html += $"<span class='text-danger ms-2'>{item.HelpUrl}</span>";
                }
            }
            else
            {
                html += "<span class='badge bg-danger'>No Action Linked to Help</span>";
            }

            html += "</div>";
            html += "<div class='mt-1'>";

            var topic = HelpTopics.FindTopicByContentActionId(item.ActionIdentifier);

            if (topic != null)
            {
                var helpUrl = ServiceLocator.AppSettings.Application.HelpUrl + topic.Url;
                var helpLink = $"<a title='View help topic' target='_blank' href='{helpUrl}'>{topic.Url}</a>";

                html += "<span class='badge bg-success'>Help Linked to Action</span>";

                html += $"<span class='text-success ms-2'>{helpLink}<a title='Edit help topic' target='_blank' href='/ui/admin/sites/pages/outline?panel=content&id={topic.Identifier}' class='ms-2'><i class='fas fa-pencil'></i></a></span>";
            }
            else
            {
                html += "<span class='badge bg-danger'>No Help Linked to Action</span>";
            }

            html += "</div>";

            return html;
        }
    }
}