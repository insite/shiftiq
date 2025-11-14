using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Admin.Jobs.Candidates.Controls
{
    public partial class SubjectCommentGrid : SearchResultsGridViewController<TCandidateCommentFilter>
    {
        public event EventHandler Refreshed;

        private void OnRefreshed() => Refreshed?.Invoke(this, EventArgs.Empty);

        protected override bool IsFinder => false;

        public bool AllowEdit
        {
            get { return (bool?)ViewState[nameof(AllowEdit)] ?? false; }
            set { ViewState[nameof(AllowEdit)] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDeleting += Grid_DeleteCommand;
        }

        protected override void OnPreRender(EventArgs e)
        {
            SetColumnsVisibility();

            base.OnPreRender(e);
        }

        private void Grid_DeleteCommand(object sender, GridViewDeleteEventArgs e)
        {
            var grid = (Grid)sender;
            var commentKey = grid.GetDataKey<Guid>(e);

            TCandidateCommentStore.Delete(commentKey);

            OnRefreshed();
        }

        public void LoadData(Guid contactId)
        {
            Search(new TCandidateCommentFilter() { SubjectContactId = contactId });
        }

        protected override IListSource SelectData(TCandidateCommentFilter filter)
        {
            return TCandidateCommentSearch.SelectSearchResults(filter);
        }

        protected override int SelectCount(TCandidateCommentFilter filter)
        {
            return TCandidateCommentSearch.CountByFilter(filter);
        }

        protected string GetModalUrl(Guid? id)
        {
            if (id.HasValue)
                return "contactCommentGrid.showEditor('" + id.Value.ToString() + "'); return false;";
            return "";
        }

        private void SetColumnsVisibility()
        {
            Grid.Columns.FindByName("Commands").Visible = AllowEdit;
        }
    }
}