using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Portal;

using Shift.Common.Linq;

namespace InSite.UI.Portal.Standards.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<StandardFilter>
    {
        #region Classes

        public class ExportDataItem
        {
            public int AssetNumber { get; set; }
            public string Code { get; set; }
            public string ContentTitle { get; set; }
            public string StandardLabel { get; set; }
        }

        private class SearchDataItem
        {
            public Guid OrganizationIdentifier { get; set; }
            public int AssetNumber { get; set; }
            public string Code { get; set; }
            public string ContentTitle { get; set; }
            public string StandardLabel { get; set; }
            public Guid StandardIdentifier { get; set; }
        }

        #endregion

        #region Initialization and Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        #endregion

        #region Methods (data binding)

        protected override int SelectCount(StandardFilter filter)
            => StandardSearch.Count(filter);

        protected string GetOutlineUrl()
        {
            var dataItem = Page.GetDataItem();
            var standardId = DataBinder.Eval(dataItem, "StandardIdentifier");
            var url = $"/ui/portal/standards/outline?standard={standardId}";

            return ((PortalBasePage)Page).AddFolderToUrl(url);
        }

        #endregion

        #region Methods (export)

        public override IListSource GetExportData(StandardFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<SearchDataItem>().Select(x => new ExportDataItem
            {
                Code = x.Code,
                AssetNumber = x.AssetNumber,
                ContentTitle = x.ContentTitle,
                StandardLabel = x.StandardLabel
            }).ToList().ToSearchResult();
        }

        protected override IListSource SelectData(StandardFilter filter)
        {
            filter.OrderBy = "ContentTitle";

            return StandardSearch.Select(filter)
                .Select(x => new SearchDataItem
                {
                    OrganizationIdentifier = x.OrganizationIdentifier,
                    AssetNumber = x.AssetNumber,
                    Code = x.Code,
                    ContentTitle = x.ContentTitle,
                    StandardLabel = x.StandardLabel,
                    StandardIdentifier = x.StandardIdentifier
                }).ToList().ToSearchResult();
        }
        #endregion
    }
}