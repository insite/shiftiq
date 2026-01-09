using System;
using System.ComponentModel;
using System.Text;

using InSite.Application.Sites.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Sites.Pages.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QPageFilter>
    {
        protected override int SelectCount(QPageFilter filter)
        {
            return ServiceLocator.PageSearch.Count(filter);
        }

        protected override IListSource SelectData(QPageFilter filter)
        {
            if (filter.OrderBy.IsEmpty())
                filter.OrderBy = "PageSlug";

            return ServiceLocator.PageSearch
                .GetPageSearchItems(filter)
                .ToSearchResult();
        }

        protected string GetHierarchyHtml()
        {
            var scope = (string)Eval("Scope");
            var parent = (Guid?)Eval("ParentPageIdentifier");
            var up = parent == null ? 0 : 1;
            var down = (int)Eval("ChildrenCount");

            var html = new StringBuilder();

            if (scope == "Root") // no parents
                html.Append($"<span>Root</span> <i class='far fa-level-down'></i> {down}");

            else if (scope == "Leaf") // no children
                html.Append($"{up} <i class='far fa-level-up'></i> <span>Leaf</span>");
            
            else // parents and children
                html.Append($"{up} <i class='far fa-level-up'></i> <span>Branch</span> <i class='far fa-level-down'></i> {down}");

            return html.ToString();
        }
    }
}