using InSite.Application.Events.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Events.Seats.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QSeatFilter>
    {
        public override QSeatFilter Filter
        {
            get
            {
                var filter = new QSeatFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    EventScheduledSince = EventScheduledSince.Value,
                    EventScheduledBefore = EventScheduledBefore.Value,
                    AchievementIdentifier = AchievementIdentifier.ValueAsGuid,
                    EventTitle = EventTitle.Text,
                    SeatTitle = SeatTitle.Text,
                    IsAvailable = IsAvailable.ValueAsBoolean,
                    IsTaxable = IsTaxable.ValueAsBoolean
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                EventScheduledSince.Value = value.EventScheduledSince;
                EventScheduledBefore.Value = value.EventScheduledBefore;
                AchievementIdentifier.ValueAsGuid = value.AchievementIdentifier;
                EventTitle.Text = value.EventTitle;
                SeatTitle.Text = value.SeatTitle;
                IsAvailable.ValueAsBoolean = value.IsAvailable;
                IsTaxable.ValueAsBoolean = value.IsTaxable;
            }
        }

        public override void Clear()
        {
            EventScheduledSince.Value = null;
            EventScheduledBefore.Value = null;
            AchievementIdentifier.ValueAsGuid = null;
            EventTitle.Text = null;
            SeatTitle.Text = null;
            IsAvailable.ClearSelection();
            IsTaxable.ClearSelection();
        }
    }
}
