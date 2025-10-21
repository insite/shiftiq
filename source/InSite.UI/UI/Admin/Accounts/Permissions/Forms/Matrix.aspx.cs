using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Sdk.UI;

namespace InSite.Admin.Accounts.Permissions.Forms
{
    public partial class Matrix : SearchPage<MatrixFilter>
    {
        private const string GroupNameColumn = "GroupName";
        private const string ToolkitsColumn = "Toolkits";

        public override string EntityName => "Permission";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchDownload.NeedVisibleColumns += SearchDownload_NeedVisibleColumns;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                PageHelper.AutoBindHeader(this);
        }

        private void SearchDownload_NeedVisibleColumns(object sender, BaseSearchDownload.NeedVisibleColumnsArgs args)
        {
            if (SearchResults.ActionItems == null
                || !args.Columns.Any(x => !x.Hidden && string.Equals(x.Info.Name, ToolkitsColumn, StringComparison.OrdinalIgnoreCase))
                )
            {
                return;
            }

            var visibleColumns = new List<DownloadColumn>();
            
            var groupNameColumn = args.Columns.FirstOrDefault(x => !x.Hidden && string.Equals(x.Info.Name, GroupNameColumn, StringComparison.OrdinalIgnoreCase))?.Info;
            if (groupNameColumn != null)
                visibleColumns.Add(groupNameColumn);

            var columns = SearchResults.ActionItems.Select(x => x.DataField).ToHashSet();
            var toolkits = TActionSearch.Search(x => x.ActionType == "Permission");

            foreach (var tool in toolkits)
            {
                var dataField = "Column" + tool.ActionIdentifier.GetHashCode();
                visibleColumns.Add(new DownloadColumn(dataField, tool.ActionName) { Visible = true });
            }

            args.VisibleColumns = visibleColumns.ToArray();
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new DownloadColumn[]
            {
                new DownloadColumn(GroupNameColumn, "Group Name"),
                new DownloadColumn(ToolkitsColumn, "Toolkits")
            };
        }
    }
}