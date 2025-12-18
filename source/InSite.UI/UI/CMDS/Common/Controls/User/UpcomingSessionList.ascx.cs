using System;
using System.Linq;
using System.Web.UI;

using InSite.Application.Events.Read;

using Shift.Constant;

namespace InSite.Custom.CMDS.User.Programs.Controls
{
    public partial class UpcomingSessionList : UserControl
    {
        public void LoadData()
        {
            var now = DateTimeOffset.UtcNow;

            var filter = new QEventFilter
            {
                OrganizationIdentifier = OrganizationIdentifiers.Keyera,
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

                return new
                {
                    Identifier = isFull ? Guid.Empty : x.EventIdentifier,
                    Title = x.EventTitle,
                    IsFull = isFull,
                    IsClosed = !isRegistrationOpen
                };
            }).ToArray();

            UpcomingSessionRepeater.Visible = list.Length > 0;

            UpcomingSessionRepeater.DataSource = list;
            UpcomingSessionRepeater.DataBind();
        }
    }
}