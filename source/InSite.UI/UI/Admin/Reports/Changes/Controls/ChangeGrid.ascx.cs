using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Reports.Changes.Controls
{
    public partial class ChangeGrid : SearchResultsGridViewController<NullFilter>
    {
        protected override bool IsFinder => false;

        protected Guid AggregateID
        {
            get => ViewState[nameof(AggregateID)] as Guid? ?? Guid.Empty;
            set => ViewState[nameof(AggregateID)] = value;
        }

        private bool IncludeChildren
        {
            get => ViewState[nameof(IncludeChildren)] as bool? ?? false;
            set => ViewState[nameof(IncludeChildren)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterButton.Click += FilterButton_Click;
            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.RegisterStartupScript(
                Page,
                GetType(),
                "init_" + ClientID,
                $"changeGrid.init();",
                true);

            base.OnPreRender(e);
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            Search(new NullFilter());
        }

        public int LoadData(Guid aggregateID)
        {
            AggregateID = aggregateID;

            var aggregate = ServiceLocator.AggregateSearch.Get(aggregateID);

            IncludeChildren = string.Equals(aggregate.AggregateType, "Gradebook");

            Search(new NullFilter());

            return RowCount;
        }

        protected override int SelectCount(NullFilter filter)
        {
            return ServiceLocator.ChangeStore.Count(AggregateID, FilterTextBox.Text, IncludeChildren);
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            var (skip, take) = filter.Paging.ToSkipTake();

            var changes = ServiceLocator.ChangeStore
                .GetChangesPaged(AggregateID, FilterTextBox.Text, IncludeChildren, skip, take);

            return ChangeRepeater.DataItem
                .FromChanges(changes, User.TimeZone)
                .ToSearchResult();
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            var changes = ServiceLocator.ChangeStore
                .GetChangesPaged(AggregateID, FilterTextBox.Text, IncludeChildren, 0, int.MaxValue);

            var list = ChangeRepeater.DataItem
                .FromChanges(changes, User.TimeZone, false)
                .ToList();

            if (list.Count == 0)
                return;

            var filename = string.Format("{0} {1:yyyy-MM-dd}",
                StringHelper.Sanitize("History", '_'), DateTime.Now);

            var helper = new CsvExportHelper(list);

            helper.AddMapping("Version", "Version");
            helper.AddMapping("Time", "Date and Time", "{0:MMM d, yyyy h:mm tt}");
            helper.AddMapping("Name", "ChangeType");
            helper.AddMapping("User", "User");
            helper.AddMapping("Data", "Data");

            var bytes = helper.GetBytes(Encoding.UTF8);

            Page.Response.SendFile(filename, "csv", bytes);
        }
    }
}