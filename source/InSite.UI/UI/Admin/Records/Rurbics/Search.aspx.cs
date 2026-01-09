using System;
using System.Collections.Generic;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Contract;

namespace InSite.UI.Admin.Records.Rurbics
{
    public partial class Search : SearchPage<QRubricFilter>
    {
        public const string NavigateUrl = "/ui/admin/records/rubrics/search";

        public static void Redirect() => HttpResponseHelper.Redirect(NavigateUrl);

        public override string EntityName => "Rubric";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("RubricIdentifier", "Identifier", null, 15, HorizontalAlignment.Right),
                new DownloadColumn("RubricTitle", "Rubric Title", null, 60),
                new DownloadColumn("RubricPoints", "Total Rubric Points", "####0.00", 15, HorizontalAlignment.Right),
                new DownloadColumn("CriteriaCount", "Number of Criteria", null, 15, HorizontalAlignment.Right),
                new DownloadColumn("Created", "Created","MMM dd, yyyy", 15)
            };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(Page, new BreadcrumbItem("Add New Rubric", Create.GetNavigateUrl(), null, null));
        }
    }
}