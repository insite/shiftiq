using System;
using System.Web.UI;

using InSite.Application.Messages.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;

namespace InSite.Admin.Messages.Contacts.Controls
{
    public partial class ContactGrid : BaseUserControl
    {
        #region Events 

        public event EventHandler Refreshed;
        private void OnRefreshed() => Refreshed?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        private Guid MessageIdentifier
        {
            get => (Guid)ViewState[nameof(MessageIdentifier)];
            set => ViewState[nameof(MessageIdentifier)] = value;
        }

        public int GroupCount
        {
            get => (int)(ViewState[nameof(GroupCount)] ?? 0);
            private set => ViewState[nameof(GroupCount)] = value;
        }

        public int PersonCount
        {
            get => (int)(ViewState[nameof(PersonCount)] ?? 0);
            private set => ViewState[nameof(PersonCount)] = value;
        }

        private string MessageType
        {
            get => ViewState[nameof(MessageType)] as string;
            set => ViewState[nameof(MessageType)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GroupGrid.Refreshed += GroupGrid_Refreshed;
            PersonGrid.EntityDeleted += PersonGrid_Refreshed;
            PersonGrid.EntityUpdated += PersonGrid_Refreshed;

            SearchInput.Click += FilterButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        #endregion

        #region Event handlers

        private void PersonGrid_Refreshed(object sender, EventArgs e)
        {
            UpdateData();
            OnRefreshed();
        }

        private void GroupGrid_Refreshed(object sender, EventArgs e)
        {
            UpdateData();
            PersonGrid.RefreshGrid();
            OnRefreshed();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            HttpResponseHelper.Redirect($"/ui/admin/messages/subscribers/delete-all?message={MessageIdentifier}");
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            GroupGrid.ContactKeyword = SearchInput.Text;
            GroupGrid.RefreshGrid();

            PersonGrid.ContactKeyword = SearchInput.Text;
            PersonGrid.RefreshGrid();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(ContactGrid),
                "filter_focus",
                $"inSite.common.baseInput.focus('{SearchInput.ClientID}_box', true);",
                true);
        }

        #endregion

        public void LoadData(Guid id, string type)
        {
            MessageIdentifier = id;
            MessageType = type;

            AddButton.NavigateUrl = $"/ui/admin/messages/subscribers/add?message={MessageIdentifier}";

            GroupGrid.LoadData(MessageIdentifier, SearchInput.Text);
            PersonGrid.LoadData(MessageIdentifier, MessageType, SearchInput.Text, null);

            UpdateData();

            DeleteButton.Visible = GroupCount + PersonCount > 0;
        }

        public void Refresh()
        {
            UpdateData();

            PersonGrid.RefreshGrid();
            GroupGrid.RefreshGrid();

            OnRefreshed();
        }

        #region Helper methods

        private void UpdateData()
        {
            var filter1 = new QSubscriberUserFilter { MessageIdentifier = MessageIdentifier };
            PersonCount = ServiceLocator.MessageSearch.CountSubscriberUsers(filter1);

            var filter2 = new QSubscriberGroupFilter { MessageIdentifier = MessageIdentifier };
            GroupCount = ServiceLocator.MessageSearch.CountSubscriberGroups(filter2);

            GroupCard.Visible = GroupCount > 0;
            PersonCard.Visible = PersonCount > 0;
            SearchPanel.Visible = GroupCount > 0 || PersonCount > 0;
        }

        #endregion
    }
}