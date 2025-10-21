using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Events.Attendees.Controls
{
    public partial class PersonAttendeePanel : SearchResultsGridViewController<QEventAttendeeFilter>
    {
        #region Events

        public event EventHandler EntityDeleted;
        private void OnEntityDeleted() => EntityDeleted?.Invoke(this, EventArgs.Empty);

        public event EventHandler EntityUpdated;
        private void OnEntityUpdated() => EntityUpdated?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        public string AggregateType
        {
            get
            {
                return ViewState[nameof(AggregateType)] as string;
            }
            set
            {
                ViewState[nameof(AggregateType)] = value;
            }
        }

        public string ContactKeyword
        {
            get => Filter?.ContactKeyword;
            set
            {
                if (Filter == null)
                {
                    return;
                }

                Filter.ContactKeyword = value;
            }
        }

        protected bool CanWrite
        {
            get => (bool)ViewState[nameof(CanWrite)];
            set => ViewState[nameof(CanWrite)] = value;
        }

        protected override bool IsFinder => false;

        #endregion

        #region Methods (loading)

        public void LoadData(Guid @event, string keyword, string role, bool canWrite)
        {
            CanWrite = canWrite;

            Search(new QEventAttendeeFilter
            {
                EventIdentifier = @event,
                ContactKeyword = keyword,
                ContactRole = role
            });
        }

        #endregion

        #region Methods (event handling)

        protected override void OnRowCommand(GridViewRow row, string name, object argument)
        {
            if (name == "DeleteContact")
            {
                var id = Grid.GetDataKey<Guid>(row);
                ServiceLocator.SendCommand(new RemoveEventAttendee(Filter.EventIdentifier.Value, id));
                SearchWithCurrentPageIndex(Filter);
                OnEntityDeleted();
            }
            else
                base.OnRowCommand(row, name, argument);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            SearchWithCurrentPageIndex(Filter);
            OnEntityUpdated();
        }

        #endregion

        #region Methods (data binding)

        protected override int SelectCount(QEventAttendeeFilter filter)
        {
            return ServiceLocator.EventSearch.CountAttendees(filter);
        }

        protected override IListSource SelectData(QEventAttendeeFilter filter)
        {
            if (filter == null)
                return null;

            return ServiceLocator.EventSearch
                .GetAttendees(filter, x => x.Person.User)
                .ToSearchResult();
        }

        #endregion
    }
}