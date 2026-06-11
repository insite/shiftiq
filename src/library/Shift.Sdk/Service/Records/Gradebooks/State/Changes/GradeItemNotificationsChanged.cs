using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradeItemNotificationsChanged : Change
    {
        public Guid Item { get; set; }
        public Notification[] Notifications { get; set; }

        public GradeItemNotificationsChanged(Guid item, Notification[] notifications)
        {
            Item = item;
            Notifications = notifications;
        }
    }
}
