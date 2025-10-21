using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Records.Reports.LearnerActivity.Models;
using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Records.Reports.LearnerActivity.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<VLearnerActivityFilter>
    {

        #region Properties

        public event EventHandler Updated;
        public event EventHandler SummaryVisibility;

        private IList<object> DataSource
        {
            get => (IList<object>)ViewState[nameof(DataSource)];
            set => ViewState[nameof(DataSource)] = value;
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            SetContentLabelHeaders(e);

            if (!IsContentItem(e.Row))
                return;
        }

        private void SetContentLabelHeaders(GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "Person Code")
                        e.Row.Cells[i].Text = LabelHelper.GetLabelContentText("Person Code");
                }
            }
        }

        #region Searching

        public override void Search(VLearnerActivityFilter filter, bool refreshLastSearched = false)
        {
            DataSource = null;

            base.Search(filter, refreshLastSearched);
        }

        protected override void Search(VLearnerActivityFilter filter, int pageIndex)
        {
            if (DataSource == null)
            {
                var dataItems = VLearnerActivitySearch.Bind(x => x, filter)
                    .OrderByDescending(x => x.LearnerCreated).ToArray();

                SummaryCounterData summaryData;

                if (filter.IsSummaryCountStrategy)
                {
                    DataSource = VLearnerActivitySearch.Summarize(dataItems)
                        .OrderByDescending(x => x.LearnerCreated).ToArray();

                    summaryData = GetLearnersSummaryCounterData(dataItems);
                }
                else
                {
                    DataSource = dataItems;

                    summaryData = GetActivitiesSummaryCounterData(dataItems);
                }

                Updated?.Invoke(this, new SummaryCounterDataEventArgs(summaryData, filter));
            }

            base.Search(filter, pageIndex);
        }

        protected override int SelectCount(VLearnerActivityFilter filter)
        {
            return DataSource.Count;
        }

        protected override IListSource SelectData(VLearnerActivityFilter filter)
        {
            return DataSource
                .ApplyPaging(filter)
                .ToList()
                .ToSearchResult();
        }

        protected override void SetGridVisibility(bool isVisible, bool showInstructions)
        {
            base.SetGridVisibility(isVisible, showInstructions);
            if(isVisible)
                SummaryVisibility?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Summary

        private SummaryCounterData GetLearnersSummaryCounterData(IEnumerable<VLearnerActivity> list)
        {
            return GetSummaryCounterData(list, x => x.Select(y => y.LearnerIdentifier).Distinct().Count());
        }

        private SummaryCounterData GetActivitiesSummaryCounterData(IEnumerable<VLearnerActivity> list)
        {
            return GetSummaryCounterData(list, x => x.Count());
        }

        private SummaryCounterData GetSummaryCounterData(IEnumerable<VLearnerActivity> list, Func<IEnumerable<VLearnerActivity>, int> getCount)
        {
            return new SummaryCounterData
            {
                ProgramNames = list.GroupBy(x => x.ProgramName)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                GradebookNames = list.GroupBy(x => x.GradebookName)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                EnrollmentStatuses = list.GroupBy(x => x.EnrollmentStatus)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                EngagementStatuses = list.GroupBy(x => x.EngagementStatus)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                LearnerGenders = list.GroupBy(x => x.LearnerGender)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                LearnerReferrers = list.GroupBy(x => x.ReferrerName)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                ImmigrationStatuses = list.GroupBy(x => x.ImmigrationStatus)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                ImmigrationDestinations = list.GroupBy(x => x.ImmigrationDestination)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                LearnerCitizenships = list.GroupBy(x => x.LearnerCitizenship)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray()
            };
        }

        #endregion

        #region Export

        public override IListSource GetExportData(VLearnerActivityFilter filter, bool empty)
        {
            if (empty)
                return new List<VLearnerActivitySummary>().ToSearchResult();

            var data = VLearnerActivitySearch.Bind(x => x, filter)
                    .OrderByDescending(x => x.LearnerCreated)
                    .ToList();

            if (!filter.IsSummaryCountStrategy)
                return data.ToSearchResult();

            return VLearnerActivitySearch.Summarize(data)
                .OrderByDescending(x => x.LearnerCreated)
                .ToList()
                .ToSearchResult();
        }

        #endregion
    }
}