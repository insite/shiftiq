using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using InSite.Admin.Records.Reports.LearnerActivity.Controls;

namespace InSite.UI.Admin.Records.Reports.LearnerActivity
{
    public partial class Search : SearchPage<VLearnerActivityFilter>
    {
        public override string EntityName => "Learner Activity";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                PageHelper.AutoBindHeader(this);
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload
                .GetColumns(typeof(SearchResults.ExportDataItem))
                .OrderBy(x => x.Name);
        }
    }
}