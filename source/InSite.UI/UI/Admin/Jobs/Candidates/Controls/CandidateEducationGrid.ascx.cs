using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Admin.Jobs.Candidates.Controls
{
    public partial class CandidateEducationGrid : SearchResultsGridViewController<NullFilter>
    {
        #region Properties

        protected override bool IsFinder => false;

        private Guid ContactId
        {
            get => (Guid)ViewState[nameof(ContactId)];
            set => ViewState[nameof(ContactId)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDeleting += Grid_DeleteCommand;

            RefreshGridButton.Click += RefreshGridButton_Click;
        }

        #endregion

        #region Event handlers

        private void Grid_DeleteCommand(object sender, GridViewDeleteEventArgs e)
        {
            var grid = (Grid)sender;
            var educationId = grid.GetDataKey<Guid>(e);

            TCandidateEducationStore.Delete(educationId);

            Search(Filter);
        }

        private void RefreshGridButton_Click(object sender, EventArgs e)
        {
            Search(Filter);
        }

        #endregion

        #region Public methods

        public void LoadData(Guid contactId)
        {
            ContactId = contactId;

            Search(new NullFilter());

            AddNewLink.Attributes["OnClick"] = "candidateEducationGrid.showCreator('" + contactId.ToString() + "'); return false;";
            AddNewLink.Visible = true;
        }

        #endregion

        #region Search results

        protected override IListSource SelectData(NullFilter filter)
        {
            var data = TCandidateEducationSearch
                .SelectByContact(ContactId)
                .OrderByDescending(x => x.EducationDateFrom)
                .ThenBy(x => x.EducationInstitution)
                .ApplyPaging(filter)
                .ToList();

            return new SearchResultList(data);
        }

        protected override int SelectCount(NullFilter filter)
        {
            return TCandidateEducationSearch.Count(x => x.UserIdentifier == ContactId);
        }

        #endregion

        #region Helpers
        protected string GetModalUrl(Guid? id)
        {
            if (id.HasValue)
                return "candidateEducationGrid.showEditor('" + id.Value.ToString() + "'); return false;";
            return "";
        }


        protected string GetDateString(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("MMM d, yyyy") : string.Empty;
        }

        #endregion
    }
}