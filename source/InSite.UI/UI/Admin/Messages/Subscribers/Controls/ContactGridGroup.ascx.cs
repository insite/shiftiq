using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.Admin.Messages.Contacts.Controls
{
    public partial class ContactGridGroup : SearchResultsGridViewController<QSubscriberGroupFilter>
    {
        #region Events

        public event EventHandler Refreshed;

        private void OnRefreshed() => Refreshed?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        protected override bool IsFinder => false;

        public string ContactKeyword
        {
            get => Filter?.SubscriberKeyword;
            set
            {
                if (Filter == null)
                {
                    Filter = new QSubscriberGroupFilter();
                }

                Filter.SubscriberKeyword = value;
            }
        }

        #endregion

        #region Methods (loading)

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDeleting += Grid_DeleteCommand;
        }

        public void LoadData(Guid aggregate, string keyword)
        {
            Search(new QSubscriberGroupFilter
            {
                MessageIdentifier = aggregate,
                SubscriberKeyword = keyword
            });
        }

        #endregion

        #region Methods (data binding)

        private void Grid_DeleteCommand(object sender, GridViewDeleteEventArgs e)
        {
            var grid = (Grid)sender;
            var groupId = grid.GetDataKey<Guid>(e);

            var people = MembershipSearch.Bind(x => x.User.UserIdentifier, x => x.Group.GroupIdentifier == groupId);
            var subscribers = ServiceLocator.MessageSearch.GetSubscriberUsers(Filter.MessageIdentifier);

            var subscriberPeople = people.Where(x => subscribers.FirstOrDefault(y => y.UserIdentifier == x) != null).ToList();

            foreach (var person in subscriberPeople)
                ServiceLocator.SendCommand(new RemoveMessageSubscriber(Filter.MessageIdentifier, person, false));

            ServiceLocator.SendCommand(new RemoveMessageSubscriber(Filter.MessageIdentifier, groupId , true));

            OnRefreshed();
        }

        protected override int SelectCount(QSubscriberGroupFilter filter)
        {
            return ServiceLocator.MessageSearch.CountSubscriberGroups(filter);
        }

        protected override IListSource SelectData(QSubscriberGroupFilter filter)
        {
            return ServiceLocator.MessageSearch
                .GetSubscriberGroups(filter)
                .ToSearchResult();
        }

        #endregion
    }
}