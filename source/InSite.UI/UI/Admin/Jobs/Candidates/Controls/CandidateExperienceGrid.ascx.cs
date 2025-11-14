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
    public partial class CandidateExperienceGrid : SearchResultsGridViewController<NullFilter>
    {
        protected override bool IsFinder => false;

        private Guid ContactId
        {
            get => (Guid)ViewState[nameof(ContactId)];
            set => ViewState[nameof(ContactId)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDeleting += Grid_DeleteCommand;

            RefreshGridButton.Click += RefreshGridButton_Click;
        }

        private void Grid_DeleteCommand(object sender, GridViewDeleteEventArgs e)
        {
            var grid = (Grid)sender;
            var experienceKey = grid.GetDataKey<Guid>(e);

            TCandidateExperienceStore.Delete(experienceKey);

            Search(Filter);
        }

        private void RefreshGridButton_Click(object sender, EventArgs e)
        {
            Search(Filter);
        }

        public void LoadData(Guid contactId)
        {
            ContactId = contactId;

            Search(new NullFilter());

            AddNewLink.Attributes["OnClick"] = "candidateExperienceGrid.showCreator('" + contactId.ToString() + "'); return false;";

            AddNewLink.Visible = true;
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            var data = TCandidateExperienceSearch
                .SelectByContact(ContactId)
                .OrderByDescending(x => x.ExperienceDateFrom)
                .ThenBy(x => x.EmployerName)
                .ApplyPaging(filter)
                .ToList();

            return new SearchResultList(data);
        }

        protected override int SelectCount(NullFilter filter)
        {
            return TCandidateExperienceSearch.Count(x => x.UserIdentifier == ContactId);
        }

        protected string GetDateString(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("MMM d, yyyy") : string.Empty;
        }

        protected string GetModalUrl(Guid? id)
        {
            if (id.HasValue)
                return "candidateExperienceGrid.showEditor('" + id.Value.ToString() + "'); return false;";
            return "";
        }
    }
}