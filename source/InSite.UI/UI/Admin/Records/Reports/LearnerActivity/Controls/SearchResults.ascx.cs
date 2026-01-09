using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
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
        #region Events

        public event EventHandler Updated;

        #endregion

        #region Properties

        internal IList<SearchResultDataItem> DataSource
        {
            get => (IList<SearchResultDataItem>)ViewState[nameof(DataSource)];
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
                for (var i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "Person Code")
                        e.Row.Cells[i].Text = LabelHelper.GetLabelContentText("Person Code");
                }
            }
        }

        protected string GetProgramsHtml()
        {
            var dataItem = (SearchResultDataItem)Page.GetDataItem();

            if (dataItem.Programs.IsEmpty())
                return null;

            if (dataItem.Programs.Length == 1)
                return HttpUtility.HtmlEncode(dataItem.Programs[0].Name);

            return
                "<ul>" +
                string.Concat(dataItem.Programs.Select(x => "<li>" + HttpUtility.HtmlEncode(x.Name) + "</li>")) +
                "</ul>";
        }

        #region Searching

        public override void Search(VLearnerActivityFilter filter, bool refreshLastSearched = false)
        {
            DataSource = null;

            base.Search(filter, refreshLastSearched);
        }

        protected override void Search(VLearnerActivityFilter filter, int pageIndex)
        {
            var isUpdated = DataSource == null;

            if (isUpdated)
            {
                var dataItems = VLearnerActivitySearch.Select(filter)
                    .OrderByDescending(x => x.LearnerCreated).ToArray();

                if (filter.IsSummaryCountStrategy)
                {
                    DataSource = VLearnerActivitySearch.Summarize(dataItems)
                        .OrderByDescending(x => x.LearnerCreated)
                        .Select(x => new SearchResultDataItem(x))
                        .ToArray();
                }
                else
                {
                    DataSource = dataItems.Select(x => new SearchResultDataItem(x)).ToArray();
                }

                var programMapping = VLearnerActivitySearch.SelectProgramEnrollments(filter)
                    .GroupBy(x => x.UserIdentifier)
                    .ToDictionary(x => x.Key, x => x.OrderBy(y => y.ProgramName).Select(y => new SearchResultProgramInfo(y)).ToArray());

                foreach (var item in DataSource)
                {
                    if (programMapping.TryGetValue(item.LearnerIdentifier, out var programs))
                        item.Programs = programs;
                }
            }

            base.Search(filter, pageIndex);

            if (isUpdated)
                Updated?.Invoke(this, EventArgs.Empty);
        }

        protected override int SelectCount(VLearnerActivityFilter filter)
        {
            return DataSource.Count;
        }

        protected override IListSource SelectData(VLearnerActivityFilter filter)
        {
            return DataSource.ApplyPaging(filter).ToList().ToSearchResult();
        }

        #endregion

        #region Export

        public override IListSource GetExportData(VLearnerActivityFilter filter, bool empty)
        {
            if (empty)
                return new List<SearchResultDataItem>().ToSearchResult();

            var data = VLearnerActivitySearch.Select(filter)
                .OrderByDescending(x => x.LearnerCreated).ToArray();

            if (!filter.IsSummaryCountStrategy)
                return data.Select(x => new SearchResultDataItem(x)).ToList().ToSearchResult();

            return VLearnerActivitySearch.Summarize(data)
                .OrderByDescending(x => x.LearnerCreated)
                .Select(x => new SearchResultDataItem(x))
                .ToList()
                .ToSearchResult();
        }

        #endregion
    }
}