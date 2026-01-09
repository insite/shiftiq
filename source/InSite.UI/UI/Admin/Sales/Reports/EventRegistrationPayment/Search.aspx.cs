using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Sales.Reports.EventRegistrationPayment
{
    public partial class Search : SearchPage<VEventRegistrationPaymentFilter>
    {
        public override string EntityName => "Event Registration Payment";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            var columns = BaseSearchDownload
                .GetColumns(typeof(SearchResults.ExportDataItem))
                .ToList();

            var learnerCodeColumn = columns.Find(x => x.Name == "LearnerCode");
            if (learnerCodeColumn != null)
                learnerCodeColumn.Title = LabelHelper.GetLabelContentText("Person Code");

            return columns;
        }
    }
}
