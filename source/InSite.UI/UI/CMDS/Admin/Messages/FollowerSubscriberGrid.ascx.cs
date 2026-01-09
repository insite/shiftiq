using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Messages.Write;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace InSite.Custom.CMDS.Admin.Messages
{
    public partial class FollowerSubscriberGrid : BaseUserControl
    {
        #region Classes

        [Serializable]
        public class DummyFilter
        {
            public Guid OrganizationIdentifier { get; set; }
            public Guid MessageIdentifier { get; set; }
            public Guid? DepartmentIdentifier { get; set; }
            public Guid FollowerIdentifier { get; set; }
            public string[] EmploymentTypes { get; set; }
        }

        #endregion

        #region Properties

        private DummyFilter Filter
        {
            get => (DummyFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.DataBinding += Grid_DataBinding;
            Grid.RowCreated += Grid_RowCreated;
        }

        #endregion

        #region Event handlers

        private void Grid_DataBinding(object sender, EventArgs e)
        {
            if (Filter == null)
                return;

            var filter = Filter;
            var paging = Paging.SetPage(Grid.PageIndex + 1, Grid.PageSize);

            var members = UserSearch.SelectGroupMembers(filter.OrganizationIdentifier, filter.DepartmentIdentifier, filter.EmploymentTypes, paging);
            var subscribers = ServiceLocator.MessageSearch.GetSubscriberUsers(filter.MessageIdentifier);
            var followers = ServiceLocator.MessageSearch.GetFollowers(filter.MessageIdentifier);

            var table = new DataTable();

            table.Columns.Add("ContactIdentifier", typeof(Guid));
            table.Columns.Add("FullName", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Following", typeof(bool));
            table.Columns.Add("Attached", typeof(DateTimeOffset));
            table.Columns.Add("Delivered", typeof(DateTimeOffset));
            table.Columns.Add("IsAttached", typeof(bool));

            foreach (var member in members)
            {
                var row = table.NewRow();

                row["ContactIdentifier"] = member.UserIdentifier;
                row["FullName"] = member.FullName;
                row["Email"] = member.Email;

                row["IsAttached"] = false;
                row["Following"] = false;

                var subscriber = subscribers.FirstOrDefault(x => x.UserIdentifier == member.UserIdentifier);

                if (subscriber != null)
                {
                    row["IsAttached"] = true;
                    row["Following"] = followers.Any(x => x.SubscriberIdentifier == subscriber.UserIdentifier && x.FollowerIdentifier == filter.FollowerIdentifier);

                    if (subscriber.Subscribed != null)
                        row["Attached"] = subscriber.Subscribed;

                    var delivered = ServiceLocator.MessageSearch.GetLastDeliveryDate(filter.MessageIdentifier, subscriber.UserIdentifier);
                    if (delivered.HasValue)
                        row["Delivered"] = delivered;
                }

                table.Rows.Add(row);
            }

            Grid.DataSource = table;
        }

        private void Grid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var followingCheckbox = (CheckBox)e.Row.FindControl("Following");
            followingCheckbox.AutoPostBack = true;
            followingCheckbox.CheckedChanged += FollowingCheckbox_CheckedChanged;
        }

        private void FollowingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            var filter = Filter;
            var followingCheckBox = (CheckBox)sender;
            var row = (GridViewRow)followingCheckBox.NamingContainer;
            var grid = (Grid)row.NamingContainer;
            var subscriberIdentifier = grid.GetDataKey<Guid>(row);

            var follower = ServiceLocator.MessageSearch.GetFollower(filter.MessageIdentifier, subscriberIdentifier, filter.FollowerIdentifier);

            if (followingCheckBox.Checked)
            {
                if (follower == null)
                {
                    var follow = new FollowSubscriber(filter.MessageIdentifier, subscriberIdentifier, filter.FollowerIdentifier);
                    ServiceLocator.SendCommand(follow);
                }
            }
            else if (follower != null)
            {
                var unfollow = new UnfollowSubscriber(filter.MessageIdentifier, subscriberIdentifier, filter.FollowerIdentifier);
                ServiceLocator.SendCommand(unfollow);
            }
        }

        #endregion

        #region Public methods

        public void LoadData(Guid organization, Guid? department, string[] employmentTypes, Guid messageIdentifier, Guid followerIdentifier)
        {
            var filter = Filter = new DummyFilter
            {
                OrganizationIdentifier = organization,
                DepartmentIdentifier = department,
                MessageIdentifier = messageIdentifier,
                FollowerIdentifier = followerIdentifier,
                EmploymentTypes = employmentTypes
            };

            Grid.PageIndex = 0;
            Grid.VirtualItemCount = UserSearch.CountGroupMembers(
                filter.OrganizationIdentifier,
                filter.DepartmentIdentifier,
                filter.EmploymentTypes);
            Grid.DataBind();
        }

        #endregion

        #region Methods (helpers)

        protected static string GetLocalTime(DateTimeOffset? date)
        {
            return date.Format(User.TimeZone, true);
        }

        #endregion
    }
}