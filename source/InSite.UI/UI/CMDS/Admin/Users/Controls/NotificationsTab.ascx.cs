using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.CMDS.Admin.Users.Controls
{
    public partial class NotificationsTab : BaseUserControl
    {
        private Guid UserIdentifier => Guid.TryParse(Request.QueryString["userID"], out var value) ? value : User.UserIdentifier;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            NotificationsGrid.EnablePaging = false;
            NotificationsGrid.DataBinding += NotificationsGrid_DataBinding;

            FollowingGrid.EnablePaging = false;
            FollowingGrid.DataBinding += FollowingGrid_DataBinding;
            FollowingGrid.RowCommand += FollowingGrid_RowCommand;
        }

        private void NotificationsGrid_DataBinding(object sender, EventArgs e)
        {
            var filter = new QSubscriberUserFilter
            {
                SubscriberIdentifiers = new[] { UserIdentifier }
            };

            var recipients = ServiceLocator.MessageSearch.GetSubscriberUsers(filter);
            var data = recipients
                .Select(x => new
                {
                    x.MessageTitle,
                    SubscribedText = x.Subscribed.Format(User.TimeZone, true)
                }
                )
                .OrderBy(x => x.MessageTitle)
                .ToArray();

            NotificationsGrid.DataSource = data;

            SubscriptionTab.Visible = data.Length > 0;

            NoDataPanel.Visible = !FollowingTab.Visible && !SubscriptionTab.Visible;
        }

        private void FollowingGrid_DataBinding(object sender, EventArgs e)
        {
            var followees = ServiceLocator.MessageSearch
                .GetFollowers(new QFollowerFilter { FollowerIdentifier = UserIdentifier })
                .OrderBy(x => x.SubscriberName)
                .ToList();

            if (followees.Count == 0)
                return;

            FollowingGrid.DataSource = followees;
            FollowingTab.Visible = followees.Count > 0;
            NoDataPanel.Visible = !FollowingTab.Visible && !SubscriptionTab.Visible;
        }

        private void FollowingGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var grid = (Grid)sender;
                var row = GridViewExtensions.GetRow(e);
                var message = grid.GetDataKey<Guid>(row, "MessageIdentifier");
                var user = grid.GetDataKey<Guid>(row, "SubscriberIdentifier");

                var unfollow = new UnfollowSubscriber(message, user, UserIdentifier);

                ServiceLocator.SendCommand(unfollow);

                FollowingGrid.DataBind();
            }
        }
    }
}