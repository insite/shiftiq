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
                EventScheduledSince = now
            };

            var events = ServiceLocator.EventSearch.GetEvents(filter, x => x.Registrations);

            var list = events.Select(x =>
            {
                var isFull = x.CapacityMaximum.HasValue && x.CapacityMaximum.Value <= x.Registrations.Count
                    || (x.CapacityMinimum ?? 0) == 0 && !x.CapacityMaximum.HasValue;

                return new
                {
                    Identifier = isFull ? Guid.Empty : x.EventIdentifier,
                    Title = x.EventTitle,
                    IsFull = isFull,
                    IsClosed = x.RegistrationDeadline.HasValue && x.RegistrationDeadline < now
                };
            }).ToArray();

            UpcomingSessionRepeater.Visible = list.Length > 0;

            UpcomingSessionRepeater.DataSource = list;
            UpcomingSessionRepeater.DataBind();
        }
    }
}