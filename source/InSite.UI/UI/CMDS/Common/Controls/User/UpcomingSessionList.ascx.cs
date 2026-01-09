using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.Custom.CMDS.User.Programs.Controls
{
    [Serializable]
    public class UpcomingSessionListItem
    {
        public DateTimeOffset At { get; set; }

        public Guid Identifier { get; set; }

        public string Title { get; set; }
        public string Format { get; set; }
        public string Badge { get; set; }

        public int? CapacityMinimum { get; set; }
        public int? CapacityMaximum { get; set; }
        public int RegistrationCount { get; set; }

        public bool IsFull { get; set; }
        public bool IsClosed { get; set; }
        public bool IsGlobal { get; set; }

        public Guid[] Registrants { get; set; } = new Guid[0];
    }

    public partial class UpcomingSessionList : BaseUserControl
    {
        public int BindModelToControls()
        {
            var list = SearchUpcomingEvents();

            var hasEvents = list.Count > 0;

            var hasOrganizationEvents = list.Count(x => !x.IsGlobal) > 0;

            EventRepeater.Visible = hasEvents;

            EventRepeater.DataSource = list;

            EventRepeater.DataBind();

            EventRegistrationItem.Visible = hasOrganizationEvents;

            return list.Count;
        }

        public static List<UpcomingSessionListItem> SearchUpcomingEvents()
        {
            var partitionId = ServiceLocator.Partition.Identifier;

            var partitionName = ServiceLocator.Partition.GetPlatformName();

            var partitionEvents = SearchUpcomingEvents(partitionId, partitionName, true);

            var organizationEvents = SearchUpcomingEvents(Organization.Identifier, Organization.Name, false);

            var list = partitionEvents.Union(organizationEvents)
                .OrderBy(x => x.At)
                .ThenBy(x => x.Title)
                .ToList();

            return list;
        }

        private static List<UpcomingSessionListItem> SearchUpcomingEvents(Guid organizationId, string organizationName, bool isGlobal)
        {
            var now = DateTimeOffset.UtcNow;

            var filter = new QEventFilter
            {
                OrganizationIdentifier = organizationId,
                EventScheduledSince = now,
                EventPublicationStatus = PublicationStatus.Published.GetDescription(),
            };

            var events = ServiceLocator.EventSearch.GetEvents(filter, x => x.Registrations);

            var list = events.Select(x =>
            {
                var hasMaximum = x.CapacityMaximum.HasValue;

                var hasMaximumRegistrations = hasMaximum && x.CapacityMaximum.Value <= x.Registrations.Count;

                var hasZeroMinimum = (x.CapacityMinimum ?? 0) == 0;

                var isFull = hasMaximumRegistrations || (!hasMaximum && hasZeroMinimum);

                var isRegistrationOpen = !x.RegistrationDeadline.HasValue || now <= x.RegistrationDeadline;

                var item = new UpcomingSessionListItem
                {
                    Identifier = isFull ? Guid.Empty : x.EventIdentifier,
                    At = x.EventScheduledStart,
                    Title = x.EventTitle,
                    Format = x.EventFormat,
                    CapacityMinimum = x.CapacityMinimum,
                    CapacityMaximum = x.CapacityMaximum,
                    IsFull = isFull,
                    IsClosed = !isRegistrationOpen,
                    IsGlobal = isGlobal,
                    RegistrationCount = x.Registrations.Count,
                    Registrants = x.Registrations.Select(r => r.CandidateIdentifier).ToArray()
                };

                item.Badge = isGlobal
                    ? $"<span class='badge bg-secondary fs-xs'>{organizationName}</span>"
                    : "";

                return item;

            }).ToList();

            return list;
        }
    }
}