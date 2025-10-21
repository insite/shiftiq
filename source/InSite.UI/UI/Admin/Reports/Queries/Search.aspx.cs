using System;
using System.Data;
using System.Text;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Reports.Queries
{
    public partial class Search : SearchPage<SqlFilter>
    {
        public override string EntityName => "Database Query";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchResults.Alert += (s, a) => ScreenStatus.AddMessage(a);
            SearchResults.Alert += (s, a) => ScreenStatus.AddMessage(a);
            SearchResults.DataUpdated += SearchResults_DataUpdated;
            SearchCriteria.DataUpdated += SearchCriteria_DataUpdated;

            DownloadButton.Click += DownloadButton_Click;
            DownloadButtonSection.Visible = SearchResults.HasData;

            PageHelper.AutoBindHeader(this);
        }

        private void SearchResults_DataUpdated(object sender, BooleanValueArgs args)
        {
            DownloadButtonSection.Visible = args.Value;
        }

        private void SearchCriteria_DataUpdated(object sender, BooleanValueArgs args)
        {
            //criteria.Visible = args.Value;
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            var data = SearchResults.ExecuteQuery(null);

            if (data == null || data.Rows.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, string.Empty, "Please select Search Criteria.");
                return;
            }

            var reportFileName = FileModel.GetNameWithoutExtension(SearchCriteria.SelectedReport);
            var filename = $"{StringHelper.Sanitize(reportFileName, '-')}";
            var helper = new CsvExportHelper(data);

            foreach (DataColumn column in data.Columns)
            {
                helper.AddMapping(column.ColumnName, column.ColumnName);
            }

            var bytes = helper.GetBytes(Encoding.UTF8);

            Page.Response.SendFile(filename, "csv", bytes);
        }
    }
}