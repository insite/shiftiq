using System;
using System.ComponentModel;

using InSite.Application.Messages.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Messages.Contacts.Controls
{
    public partial class ContactGridPerson : SearchResultsGridViewController<QSubscriberUserFilter>
    {
        #region Events

        public event EventHandler GridFiltered;
        private void OnGridFiltered() => GridFiltered?.Invoke(this, EventArgs.Empty);

        public event EventHandler EntityDeleted;
        private void OnEntityDeleted() => EntityDeleted?.Invoke(this, EventArgs.Empty);

        public event EventHandler EntityUpdated;
        private void OnEntityUpdated() => EntityUpdated?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        public string ContactKeyword
        {
            get => Filter?.SubscriberKeyword;
            set
            {
                if (Filter == null)
                {
                    return;
                }

                Filter.SubscriberKeyword = value;
            }
        }

        protected override bool IsFinder => false;

        protected bool IsNotification() => MessageType == MessageTypeName.Notification || MessageType == MessageTypeName.Alert;

        private string MessageType
        {
            get => ViewState[nameof(MessageType)] as string;
            set => ViewState[nameof(MessageType)] = value;
        }

        #endregion

        #region Methods (loading)

        public void LoadData(Guid messageIdentifier, string messageType, string keyword, string role)
        {
            MessageType = messageType;

            Search(new QSubscriberUserFilter
            {
                MessageIdentifier = messageIdentifier,
                SubscriberKeyword = keyword
            });
        }

        #endregion

        #region Methods (event handling)

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            SearchWithCurrentPageIndex(Filter);
            OnEntityUpdated();
        }

        #endregion

        #region Methods (data binding)

        protected override int SelectCount(QSubscriberUserFilter filter)
        {
            return ServiceLocator.MessageSearch.CountSubscriberUsers(filter);
        }

        protected override IListSource SelectData(QSubscriberUserFilter filter)
        {
            if (filter == null)
                return null;

            return ServiceLocator.MessageSearch
                .GetSubscriberUsers(filter)
                .ToSearchResult();
        }

        #endregion
    }
}