using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

namespace InSite.Application.Gradebooks.Write
{
    public class ChangeGradeItemNotifications : Command
    {
        public ChangeGradeItemNotifications(Guid record, Guid item, Notification[] notifications)
        {
            AggregateIdentifier = record;
            Item = item;
            Notifications = notifications;
        }

        public Guid Item { get; set; }
        public Notification[] Notifications { get; set; }
    }
}
