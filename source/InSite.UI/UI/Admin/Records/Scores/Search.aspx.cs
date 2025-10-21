using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Records.Scores.Controls;
using InSite.Application.Records.Read;
using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Records.Scores
{
    public partial class Search : SearchPage<QProgressFilter>
    {
        public override string EntityName => "Learner Score";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            var columns = BaseSearchDownload
                .GetColumns(typeof(SearchResults.ExportDataItem))
                .OrderBy(x => x.Name)
                .ToList();

            var percentColumn = columns.Find(x => x.Name == "ProgressPercent");
            if (percentColumn != null)
                percentColumn.Format = "0.0%";

            var learnerCodeColumn = columns.Find(x => x.Name == "LearnerCode");
            if (learnerCodeColumn != null)
                learnerCodeColumn.Title = LabelHelper.GetLabelContentText("Person Code");

            return columns;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                PageHelper.AutoBindHeader(this);
        }
    }
}