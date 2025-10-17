using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupStatusModified : Change
    {
        public Guid? StatusId { get; }

        public GroupStatusModified(Guid? statusId)
        {
            StatusId = statusId;
        }
    }
}
