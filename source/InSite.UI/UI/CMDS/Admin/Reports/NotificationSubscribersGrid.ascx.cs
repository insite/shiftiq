using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using static InSite.Cmds.Admin.Reports.Forms.NotificationSubscribers;

namespace InSite.Custom.CMDS.Admin.Reports.Controls
{
    public partial class NotificationsGrid : BaseUserControl
    {
        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            NotificationRepeater.ItemCreated += NotificationRepeater_ItemCreated;
            NotificationRepeater.ItemDataBound += NotificationRepeater_ItemDataBound;
        }

        #endregion

        #region Event handlers

        private void NotificationRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var subscriberRepeater = (Repeater)e.Item.FindControl("SubscriberRepeater");
            subscriberRepeater.ItemDataBound += SubscriberRepeater_ItemDataBound;
        }

        private void NotificationRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var notification = (Notification)e.Item.DataItem;

            var subscriberRepeater = (Repeater)e.Item.FindControl("SubscriberRepeater");
            subscriberRepeater.DataSource = DataBinder.Eval(notification, "Subscribers");
            subscriberRepeater.DataBind();
        }

        private void SubscriberRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var subscriber = (Subscriber)e.Item.DataItem;

            var followerRepeater = (Repeater)e.Item.FindControl("FollowerRepeater");
            followerRepeater.DataSource = subscriber.Followers;
            followerRepeater.DataBind();
        }

        #endregion

        #region Public methods

        public void LoadData(List<Notification> data)
        {
            NotificationRepeater.DataSource = data;
            NotificationRepeater.DataBind();
        }

        #endregion
    }
}