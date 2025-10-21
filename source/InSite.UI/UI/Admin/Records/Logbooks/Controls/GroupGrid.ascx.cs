using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using InSite.Application.JournalSetups.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.UI.Admin.Records.Logbooks.Controls
{
    public partial class GroupGrid : SearchResultsGridViewController<QJournalSetupGroupFilter>
    {

        public event EventHandler Refreshed;

        private void OnRefreshed() => Refreshed?.Invoke(this, EventArgs.Empty);

        protected override bool IsFinder => false;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDeleting += Grid_DeleteCommand;
        }

        public bool LoadData(Guid journalSetupId, string keyword)
        {
            Search(new QJournalSetupGroupFilter
            {
                JournalSetupIdentifier = journalSetupId,
                GroupName = keyword
            });

            return HasRows;
        }

        private void Grid_DeleteCommand(object sender, GridViewDeleteEventArgs e)
        {
            var grid = (Grid)sender;
            var groupId = grid.GetDataKey<Guid>(e);

            ServiceLocator.SendCommand(new RemoveJournalSetupGroup(Filter.JournalSetupIdentifier, groupId));

            OnRefreshed();
        }

        protected override int SelectCount(QJournalSetupGroupFilter filter)
        {
            return ServiceLocator.JournalSearch.CountJournalSetupGroups(filter);
        }

        protected override IListSource SelectData(QJournalSetupGroupFilter filter)
        {
            return ServiceLocator.JournalSearch
                .GetJournalSetupGroupDetails(filter)
                .ToSearchResult();
        }
    }
}