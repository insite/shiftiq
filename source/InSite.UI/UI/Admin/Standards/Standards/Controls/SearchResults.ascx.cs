using System;
using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Standards.Standards.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<StandardFilter>
    {
        protected bool CanDelete { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RefreshButton.Click += RefreshButton_Click;
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            SearchWithCurrentPageIndex(Filter);
        }

        protected override int SelectCount(StandardFilter filter)
        {
            return StandardSearch.Count(filter);
        }

        protected override IListSource SelectData(StandardFilter filter)
        {
            CanDelete = Identity.IsGranted(Route.ToolkitNumber, PermissionOperation.Delete);

            filter.OrderBy = "StandardType,ContentTitle";

            return StandardSearch.Bind(
                x => new
                {
                    StandardIdentifier = x.StandardIdentifier,
                    ParentStandardIdentifier = x.ParentStandardIdentifier,
                    AssetNumber = x.AssetNumber,
                    StandardType = x.StandardType,
                    StandardTier = x.StandardTier,
                    StandardLabel = x.StandardLabel,
                    StandardStatus = x.StandardStatus,
                    Code = x.Code,
                    StandardHook = x.StandardHook,
                    ContentName = x.ContentName,
                    ContentTitle = x.ContentTitle,
                    ChildCount = x.Children.Count,
                    ParentContentTitle = x.Parent.ContentTitle,
                    ParentChildCount = x.Parent.Children.Count,
                },
                filter).ToSearchResult();
        }

        protected static string GetLocalDateTime(DateTimeOffset value) =>
            value.Format(User.TimeZone);

        protected string GetHierarchyHtml()
        {
            var parentKey = (Guid?)Eval("ParentStandardIdentifier");
            var childCount = (int)Eval("ChildCount");

            if (!parentKey.HasValue)
                return $"<span class='badge bg-custom-default'>Framework</span> {childCount:n0} <i class='far fa-level-down'></i>";
            else if (childCount == 0)
                return $"1 <i class='far fa-level-up'></i> <span class='badge bg-custom-default'>Competency</span>";
            else // Folder
                return $"1 <i class='far fa-level-up'></i> <span class='badge bg-custom-default'>Cluster</span> <i class='far fa-level-down'></i> {childCount}";
        }
    }
}