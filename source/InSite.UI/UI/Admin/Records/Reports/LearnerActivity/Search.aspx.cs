using System;

using InSite.Admin.Records.Reports.LearnerActivity.Models;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common.Events;

namespace InSite.UI.Admin.Records.Reports.LearnerActivity
{
    public partial class Search : SearchPage<VLearnerActivityFilter>
    {
        public override string EntityName => "Learner Activity";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchResults.Updated += SearchResults_Updated;
            SearchResults.DataStateChanged += SearchResults_DataStateChanged;

            SummaryTables.NeedDataSource += SummaryTables_NeedDataSource;

            PageHelper.AutoBindHeader(this);
        }

        private void SearchResults_DataStateChanged(object sender, BooleanValueArgs args)
        {
            SummaryTab.Visible = args.Value;
        }

        private void SearchResults_Updated(object sender, EventArgs e)
        {
            SummaryTables.BindSummaryCounterData();
        }

        private void SummaryTables_NeedDataSource(object sender, SummaryTablesNeedDataSourceArgs args)
        {
            args.DataSource = new SummaryTablesDataSource(
                SearchResults.DataSource, 
                SearchResults.Filter.IsSummaryCountStrategy);
        }
    }
}