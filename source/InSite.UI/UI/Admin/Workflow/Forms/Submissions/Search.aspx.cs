using System;
using System.Collections.Generic;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

namespace InSite.Admin.Workflow.Forms.Submissions
{
    public partial class Search : SearchPage<QResponseSessionFilter>
    {
        public override string EntityName
            => "Form Submission";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("ResponseSessionIdentifier", "ResponseIdentifier", null, 40),
                new DownloadColumn("ResponseSessionStarted", "ResponseStarted", "MMM dd, yyyy", 15),
                new DownloadColumn("ResponseSessionCompleted", "ResponseCompleted", "MMM dd, yyyy", 15),
                new DownloadColumn("GroupName", "Group", null, 15),
                new DownloadColumn("PeriodName", "Period", null, 15),

                new DownloadColumn("SurveyFormIdentifier", "SurveyIdentifier", null, 40),
                new DownloadColumn("SurveyNumber", "SurveyNumber", null, 15),
                new DownloadColumn("SurveyName", "SurveyName", null, 40),

                new DownloadColumn("RespondentUserIdentifier", "RespondentIdentifier", null, 40),
                new DownloadColumn("RespondentName", "RespondentName", null, 40),
                new DownloadColumn("RespondentEmail", "RespondentEmail", null, 40),

                new DownloadColumn("FirstSelection", "FirstSelection", null, 50),
                new DownloadColumn("FirstComment", "FirstComment", null, 50)
            };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            PageHelper.AutoBindHeader(this);
        }
    }
}