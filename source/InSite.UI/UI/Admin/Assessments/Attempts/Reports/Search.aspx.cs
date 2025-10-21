using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Assessments.Attempts.Reports
{
    public partial class Search : SearchPage<QAttemptFilter>
    {
        public override string EntityName => "Attempt";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("OccupationTitle", "Occupation Title", null, 45),
                new DownloadColumn("FrameworkTitle", "Framework Title", null, 45),
                new DownloadColumn("FormName", "Form Name", null, 45),
                new DownloadColumn("FormAssetVersion", "Form Version", null, 15),
                new DownloadColumn("FormAsset", "Form Asset", null, 15),
                new DownloadColumn("CandidateFirstName", "Candidate First Name", null, 25),
                new DownloadColumn("CandidateLastName", "Candidate Last Name", null, 25),
                new DownloadColumn("CandidateCode", "Candidate Code", null, 15),
                new DownloadColumn("AttemptStarted", "Attempt Started", null, 20, HorizontalAlignment.Right),
                new DownloadColumn("AttemptGraded", "Attempt Graded", null, 20, HorizontalAlignment.Right),
                new DownloadColumn("AttemptDuration", "Attempt Duration", null, 20, HorizontalAlignment.Right),
                new DownloadColumn("AttemptScore", "Attempt Score", null, 15, HorizontalAlignment.Right),
                new DownloadColumn("AttemptGrade", "Attempt Grade", null, 15),
                new DownloadColumn("AttemptTag", "Attempt Tag", null, 30),
                new DownloadColumn("EventFormat", "Event Format", null, 15),
                new DownloadColumn("GradingAssessor", "Grading Assessor", null, 50),
            };
        }

        protected override IList GetExportData(int? take = null)
        {
            return SearchResults.GetExportData(take).GetList().Cast<QAttempt>()
                .Select(x => new
                {
                    x.Form?.VBank?.OccupationTitle,
                    x.Form?.VBank?.FrameworkTitle,
                    x.Form?.FormName,
                    x.Form?.FormAsset,
                    x.Form?.FormAssetVersion,
                    CandidateFirstName = x.LearnerPerson?.UserFirstName,
                    CandidateLastName = x.LearnerPerson?.UserLastName,
                    CandidateCode = x.LearnerPerson?.PersonCode,
                    x.AttemptStarted,
                    x.AttemptGraded,
                    x.AttemptDuration,
                    x.AttemptScore,
                    x.AttemptGrade,
                    x.AttemptTag,
                    x.Registration?.Event?.EventFormat,
                    GradingAssessor = x.GradingAssessor?.UserFullName
                })
                .ToArray();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);

            SearchResults.AssessorAssigned += SearchResults_OwnerAssigned;
            SearchResults.AssessorUnassigned += SearchResults_OwnerUnassigned;
        }

        private void SearchResults_OwnerAssigned(object sender, EventArgs e)
        {
            ScreenStatus.AddMessage(AlertType.Success, "Assessor was assigned for selected attempts");

            SearchResults.SearchWithCurrentPageIndex(SearchResults.Filter);
        }

        private void SearchResults_OwnerUnassigned(object sender, EventArgs e)
        {
            ScreenStatus.AddMessage(AlertType.Success, "Assessor was unassigned for selected attempts");

            SearchResults.SearchWithCurrentPageIndex(SearchResults.Filter);
        }
    }
}