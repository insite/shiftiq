using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Portal.Standards.Documents.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<StandardDocumentFilter>
    {
        public class ExportDataItem
        {
            public DateTimeOffset? Posted { get; set; }
            public string Title { get; set; }
            public string Document { get; set; }
            public string Template { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowCommand += Grid_RowCommand;
        }

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "DeleteDocument":
                    var standardIdentifier = Guid.Parse(e.CommandArgument as string);
                    StandardStore.Delete(standardIdentifier);
                    RefreshGrid();
                    break;
            }
        }

        protected void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = e.Row.DataItem;
            if (dataItem != null)
            {
                bool isTemplate = (bool)DataBinder.Eval(dataItem, "IsTemplate");
                if (isTemplate)
                    Grid.Columns[Grid.Columns.Count - 1].Visible = false;
            }
        }

        protected override int SelectCount(StandardDocumentFilter filter)
            => StandardSearch.Count(filter);

        public override IListSource GetExportData(StandardDocumentFilter filter, bool empty)
        {
            var data = SelectData(filter).GetList();
            var result = new List<ExportDataItem>();

            foreach (object row in data)
            {
                var item = new ExportDataItem
                {
                    Document = Translate((string)DataBinder.Eval(row, "DocumentType")),
                    Title = (string)DataBinder.Eval(row, "TranslatedTitle"),
                    Posted = (DateTimeOffset?)DataBinder.Eval(row, "DatePosted"),
                    Template = (bool)DataBinder.Eval(row, "IsTemplate") ? Translate("Yes") : Translate("No")
                };

                result.Add(item);
            }

            return result.ToSearchResult();
        }

        protected override IListSource SelectData(StandardDocumentFilter filter)
        {
            return StandardSearch.SelectSearchResults(filter, Identity.Language);
        }

        protected string GetOutlineUrl()
        {
            var dataItem = Page.GetDataItem();
            var standardId = DataBinder.Eval(dataItem, "StandardIdentifier");
            var url = $"/ui/portal/standards/documents/outline?standard={standardId}";

            return ((PortalBasePage)Page).AddFolderToUrl(url);
        }

        protected static string GetDateString(DateTimeOffset? date)
        {
            return date.HasValue
                ? TimeZones.FormatDateOnly(date.Value, User.TimeZone, CultureInfo.GetCultureInfo(Identity.Language))
                : string.Empty;
        }
    }
}