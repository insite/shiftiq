using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationAnnouncement : Command, IHasRun
    {
        public string Announcement { get; set; }

        public ModifyOrganizationAnnouncement(Guid organizationId, string announcement)
        {
            AggregateIdentifier = organizationId;
            Announcement = announcement;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            var announcement = Announcement.NullIfEmpty();
            if (state.AccountWarning.NullIfEmpty() == announcement)
                return true;

            aggregate.Apply(new OrganizationAnnouncementModified(announcement));

            return true;
        }
    }
}
