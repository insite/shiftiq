using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class UserDisconnected : Change
    {
        public Guid ToUserId { get; set; }

        public UserDisconnected(Guid toUserId)
        {
            ToUserId = toUserId;
        }
    }
}
