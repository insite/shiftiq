using System;
using System.Web.UI;

using InSite.Persistence;

namespace InSite.UI.Admin.Jobs.Candidates.Controls
{
    public partial class ContactCommentGrid : UserControl
    {
        public event EventHandler Refreshed;

        private Guid ContactId
        {
            get => (Guid)ViewState[nameof(ContactId)];
            set => ViewState[nameof(ContactId)] = value;
        }

        public int AuthorCount
        {
            get => (int)(ViewState[nameof(AuthorCount)] ?? 0);
            private set => ViewState[nameof(AuthorCount)] = value;
        }

        public int SubjectCount
        {
            get => (int)(ViewState[nameof(SubjectCount)] ?? 0);
            private set => ViewState[nameof(SubjectCount)] = value;
        }

        public bool AllowEdit
        {
            get
            {
                return AuthorGrid.AllowEdit && SubjectGrid.AllowEdit;
            }
            set
            {
                AuthorGrid.AllowEdit = value;
                SubjectGrid.AllowEdit = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AuthorGrid.Refreshed += Grid_Refreshed;
            SubjectGrid.Refreshed += Grid_Refreshed;

            RefreshButton.Click += RefreshButton_Click;
        }

        protected override void OnPreRender(EventArgs e)
        {
            SetColumnsVisibility();

            base.OnPreRender(e);
        }

        private void Grid_Refreshed(object sender, EventArgs e)
        {
            Refresh();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        public void LoadData(User contact)
        {
            ContactId = contact.UserIdentifier;

            AuthorGridTitle.InnerText = $"Posted by {contact.FirstName} {contact.LastName}";
            SubjectGridTitle.InnerText = $"Posted about {contact.FirstName} {contact.LastName}";

            AddButton.OnClientClick = $"contactCommentGrid.showCreator('{ContactId}'); return false;";

            AuthorGrid.LoadData(ContactId);
            SubjectGrid.LoadData(ContactId);

            OnRefresh();
        }

        private void Refresh()
        {
            OnRefresh();

            AuthorGrid.RefreshGrid();
            SubjectGrid.RefreshGrid();
        }

        private void OnRefresh()
        {
            AuthorCount = TCandidateCommentSearch.Count(x => x.AuthorUserIdentifier == ContactId);
            SubjectCount = TCandidateCommentSearch.Count(x => x.CandidateUserIdentifier == ContactId);

            AuthorGridPanel.Visible = AuthorCount > 0;
            SubjectGridPanel.Visible = SubjectCount > 0;

            AllowEdit = true;

            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        private void SetColumnsVisibility()
        {
            ButtonPanel.Visible = AllowEdit;
            CreatorWindow.Visible = AllowEdit;
            ScriptLiteral.Visible = AllowEdit;
        }
    }
}