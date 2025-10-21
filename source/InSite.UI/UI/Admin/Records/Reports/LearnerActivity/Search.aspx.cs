using System;

using InSite.Admin.Records.Reports.LearnerActivity.Models;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Records.Reports.LearnerActivity
{
    public partial class Search : SearchPage<VLearnerActivityFilter>
    {
        public override string EntityName => "Learner Activity";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchResults.Updated += Summary_Updated;
            SearchResults.SummaryVisibility += Summary_SummaryVisibility;
            PageHelper.AutoBindHeader(this);
        }

        private void Summary_Updated(object sender, EventArgs e)
        {
            SummaryTables.BindSummaryCounterData(((SummaryCounterDataEventArgs)e).SummaryCounterData, ((SummaryCounterDataEventArgs)e).Filter);
        }

        private void Summary_SummaryVisibility(object sender, EventArgs e)
        {
            SummaryTab.Visible = true;
        }
    }
}