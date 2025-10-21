using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace InSite.Admin.Contacts.People.Controls
{
    public partial class UserConnectionGrid : SearchResultsGridViewController<NullFilter>
    {
        #region Classes

        public class LabelInfo
        {
            public string Category { get; }
            public string Title { get; }

            public LabelInfo(string category, string title)
            {
                Category = category;
                Title = title;
            }
        }

        public class ConnectionInfo
        {
            public Guid FromUserIdentifier { get; set; }
            public string FromUserFullName { get; set; }
            public string FromUserEmail { get; set; }

            public Guid ToUserIdentifier { get; set; }
            public string ToUserFullName { get; set; }
            public string ToUserEmail { get; set; }

            public bool IsManager { get; set; }
            public bool IsSupervisor { get; set; }
            public bool IsValidator { get; set; }

            public IEnumerable<LabelInfo> GetAttributes()
            {
                if (IsManager)
                    yield return new LabelInfo("custom-default", "Manager");

                if (IsSupervisor)
                    yield return new LabelInfo("custom-default", "Supervisor");

                if (IsValidator)
                    yield return new LabelInfo("custom-default", "Validator");
            }
        }

        #endregion

        #region Events

        public event EventHandler Refreshed;

        private void OnRefreshed() =>
            Refreshed?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        protected override bool IsFinder => false;

        private Guid UserIdentifier
        {
            get => (Guid)ViewState[nameof(UserIdentifier)];
            set => ViewState[nameof(UserIdentifier)] = value;
        }

        private ConnectionDirection Direction
        {
            get => (ConnectionDirection)ViewState[nameof(Direction)];
            set => ViewState[nameof(Direction)] = value;
        }

        private HashSet<Guid> SelectedUsers => (HashSet<Guid>)(ViewState[nameof(SelectedUsers)]
            ?? (ViewState[nameof(SelectedUsers)] = new HashSet<Guid>()));

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RefreshButton.Click += RefreshButton_Click;

            DeleteConnectionButton.Click += DeleteConnectionButton_Click;

            Grid.RowDataBound += Grid_ItemDataBound;
            Grid.RowCreated += Grid_ItemCreated;

            FooterScript.ContentKey = typeof(UserConnectionGrid).FullName;
        }

        #endregion

        #region Loading

        public void LoadData(QPerson person, ConnectionDirection direction, bool canEdit)
        {
            UserIdentifier = person.UserIdentifier;
            Direction = direction;

            var isOutgoingDirection = direction == ConnectionDirection.Outgoing;

            CommandButtons.Visible = canEdit && isOutgoingDirection;

            Grid.Columns.FindByName("Select").Visible = canEdit && isOutgoingDirection;
            Grid.Columns.FindByName("Commands").Visible = canEdit && isOutgoingDirection;

            Search(new NullFilter());

            AddConnectionButton.NavigateUrl = $"/ui/admin/contacts/people/create-user-connection?user={person.UserIdentifier}";
            SelectAllButton.OnClientClick = $"return userConnection.selectAll('{Grid.ClientID}','{SelectAllButton.ClientID}','{UnselectAllButton.ClientID}');";
            UnselectAllButton.OnClientClick = $"return userConnection.unselectAll('{Grid.ClientID}','{SelectAllButton.ClientID}','{UnselectAllButton.ClientID}');";
        }

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.RegisterStartupScript(
                Page, GetType(),
                "init_select_all_" + ClientID,
                $"if (typeof userConnection != 'undefined') {{ userConnection.initSelectAll('{Grid.ClientID}','{SelectAllButton.ClientID}','{UnselectAllButton.ClientID}'); }}",
                true);

            PreDeleteButton.OnClientClick = $"if (confirm('Are you sure you want to delete selected connections?')) __doPostBack('{DeleteConnectionButton.UniqueID}', ''); return false;";

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void Grid_ItemCreated(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var isSelected = (CheckBox)e.Row.FindControl("IsSelected");
            isSelected.CheckedChanged += IsSelected_CheckedChanged;
        }

        private void Grid_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var connection = (ConnectionInfo)e.Row.DataItem;

            var relationshipRepeater = (Repeater)e.Row.FindControl("RelationshipRepeater");
            relationshipRepeater.DataSource = connection.GetAttributes();
            relationshipRepeater.DataBind();

            var isSelected = (CheckBox)e.Row.FindControl("IsSelected");
            isSelected.Checked = SelectedUsers.Contains(connection.ToUserIdentifier);
        }

        private void IsSelected_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            var row = (GridViewRow)chk.NamingContainer;
            var grid = (Grid)row.NamingContainer;
            var userKey = grid.GetDataKey<Guid>(row);

            if (chk.Checked)
            {
                if (!SelectedUsers.Contains(userKey))
                    SelectedUsers.Add(userKey);
            }
            else
            {
                if (SelectedUsers.Contains(userKey))
                    SelectedUsers.Remove(userKey);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            SearchWithCurrentPageIndex(new NullFilter());

            OnRefreshed();
        }

        private void DeleteConnectionButton_Click(object sender, EventArgs e)
        {
            if (Direction != ConnectionDirection.Outgoing || SelectedUsers.Count < 0)
                return;

            foreach (Guid toId in SelectedUsers)
                UserConnectionStore.Delete(UserIdentifier, toId);

            SelectedUsers.Clear();

            SearchWithCurrentPageIndex(new NullFilter());

            OnRefreshed();
        }

        #endregion

        #region Search results

        protected override int SelectCount(NullFilter filter)
        {
            var filterExpression = GetFilterExpression();
            var count = UserConnectionSearch.Count(filterExpression);
            var hasData = count > 0;

            SelectAllButton.Visible = hasData;
            UnselectAllButton.Visible = hasData;
            DeleteConnectionButton.Visible = hasData;

            return count;
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            var filterExpression = GetFilterExpression();

            var data = UserConnectionSearch.Bind(
                x => new ConnectionInfo
                {
                    FromUserIdentifier = x.FromUser.UserIdentifier,
                    FromUserFullName = x.FromUser.FullName,
                    FromUserEmail = x.FromUser.Email,

                    ToUserIdentifier = x.ToUser.UserIdentifier,
                    ToUserFullName = x.ToUser.FullName,
                    ToUserEmail = x.ToUser.Email,

                    IsManager = x.IsManager,
                    IsSupervisor = x.IsSupervisor,
                    IsValidator = x.IsValidator,
                },
                filterExpression,
                filter.Paging,
                nameof(ConnectionInfo.FromUserFullName) + "," + nameof(ConnectionInfo.ToUserFullName)
            ).ToList();

            return data.ToSearchResult();
        }

        private Expression<Func<UserConnection, bool>> GetFilterExpression()
        {
            Expression<Func<UserConnection, bool>> filterExpression;

            if (Direction == ConnectionDirection.Incoming)
                filterExpression = x => x.ToUserIdentifier == UserIdentifier;
            else if (Direction == ConnectionDirection.Outgoing)
                filterExpression = x => x.FromUserIdentifier == UserIdentifier;
            else
                throw new NotImplementedException();

            return filterExpression;
        }

        #endregion
    }
}