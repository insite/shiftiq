using System;
using System.Linq;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Classes.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QEventFilter>
    {
        public override QEventFilter Filter
        {
            get
            {
                var filter = new QEventFilter
                {
                    EventType = "Class",
                    OrganizationIdentifier = Organization.Identifier,

                    EventScheduledSince = EventScheduledSince.Value,
                    EventScheduledBefore = EventScheduledBefore.Value,
                    EventTitle = EventTitle.Text,
                    CommentKeyword = CommentKeyword.Text,
                    Venue = Venue.Text,
                    AchievementIdentifier = AchievementIdentifier.ValueAsGuid,
                    EventInstructorIdentifier = Instructor.ValueAsGuid,
                    EventClassStatuses = EventStatus.Values.Any()
                        ? EventStatus.Values.Select(x => x.ToEnum<EventClassStatus>()).ToArray()
                        : null,
                    IsRegistrationLocked = !string.IsNullOrEmpty(IsRegistrationLocked.Value)
                        ? IsRegistrationLocked.Value == "Locked"
                        : (bool?)null,
                    PermissionGroupIdentifier = GroupIdentifier.Value,
                    Availability = !string.IsNullOrEmpty(Availability.Value)
                        ? Availability.Value.ToEnum<EventAvailabilityType>()
                        : (EventAvailabilityType?)null
                };

                GetCheckedShowColumns(filter);

                filter.OrderBy = SortColumns.Value;

                return filter;
            }
            set
            {
                EventScheduledSince.Value = value.EventScheduledSince;
                EventScheduledBefore.Value = value.EventScheduledBefore;
                EventTitle.Text = value.EventTitle;
                CommentKeyword.Text = CommentKeyword.Text;
                Venue.Text = value.Venue;
                AchievementIdentifier.ValueAsGuid = value.AchievementIdentifier;
                Instructor.ValueAsGuid = Filter.EventInstructorIdentifier;
                EventStatus.Values = value.EventClassStatuses != null
                    ? value.EventClassStatuses.Select(x => x.GetName()).ToArray()
                    : null;
                IsRegistrationLocked.Value = value.IsRegistrationLocked.HasValue
                    ? (value.IsRegistrationLocked.Value ? "Locked" : "Unlocked")
                    : null;
                GroupIdentifier.Value = value.PermissionGroupIdentifier;

                SortColumns.Value = value.OrderBy;
                Availability.Value = value.Availability.HasValue ? value.Availability.Value.GetName() : null;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            EventStatus.LoadItems(
                EventClassStatus.Drafted,
                EventClassStatus.Published,
                EventClassStatus.InProgress,
                EventClassStatus.Completed,
                EventClassStatus.Closed,
                EventClassStatus.Cancelled
            );

            Instructor.LoadItems(
                ServiceLocator.EventSearch.GetAttendeeUsers(Organization.OrganizationIdentifier, "Instructor"),
                "UserIdentifier", "UserFullName"
            );

            GroupIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;
            GroupIdentifier.Filter.MustHavePermissions = true;

            var orderedAvailabilities = new[]
            {
                EventAvailabilityType.Empty,
                EventAvailabilityType.Full,
                EventAvailabilityType.Open,
                EventAvailabilityType.Over,
                EventAvailabilityType.Under
            };

            Availability.LoadItems(
                orderedAvailabilities.Select(x => new { Value = x.GetName(), Text = x.GetName() }),
                "Value",
                "Text"
            );
        }

        public override void Clear()
        {
            EventScheduledSince.Value = null;
            EventScheduledBefore.Value = null;
            EventTitle.Text = null;
            CommentKeyword.Text = null;
            Venue.Text = null;
            AchievementIdentifier.ValueAsGuid = null;
            IsRegistrationLocked.Value = null;
            GroupIdentifier.Value = null;
            Availability.Value = null;

            Instructor.ClearSelection();
            EventStatus.ClearSelection();
        }
    }
}