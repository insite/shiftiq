using System;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common.Events;

namespace InSite.UI.Admin.Assessments.Attempts.Questions
{
    public partial class Search : SearchPage<QAttemptQuestionFilter>
    {
        public override string EntityName => "Attempt";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);

            SearchResults.DataStateChanged += SearchResults_DataStateChanged;
        }

        private void SearchResults_DataStateChanged(object sender, BooleanValueArgs args)
        {
            SearchResultsTab.Visible = args.Value;
        }
    }
}