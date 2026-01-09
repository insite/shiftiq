using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI;

using Humanizer;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Events.Classes.Controls;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Events.Classes.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QEventFilter>
    {
        protected override int SelectCount(QEventFilter filter)
        {
            return ServiceLocator.EventSearch.CountEvents(filter);
        }

        protected override IListSource SelectData(QEventFilter filter)
        {
            return GetEvents(filter).ToSearchResult();
        }

        private static List<QEvent> GetEvents(QEventFilter filter)
        {
            return ServiceLocator.EventSearch
                .GetEvents(
                    filter,
                    x => x.Achievement,
                    x => x.Registrations,
                    x => x.VenueLocation,
                    x => x.GradebookEvents.Select(y => y.Gradebook),
                    x => x.EventGroupPermissions.Select(y => y.Group),
                    x => x.Attendees.Select(y => y.Person.User)
                );
        }

        #region Export

        public class ExportDataItem
        {
            public Guid? AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public string EventDate { get; set; }
            public Guid EventIdentifier { get; set; }
            public string EventPublicationStatus { get; set; }
            public DateTimeOffset? EventScheduledEnd { get; set; }
            public DateTimeOffset EventScheduledStart { get; set; }
            public string EventSchedulingStatus { get; set; }
            public string EventTime { get; set; }
            public string EventTitle { get; set; }
            public string EventType { get; set; }
            public EventAvailabilityType Availability { get; set; }
            public int? CapacityMaximum { get; set; }
            public int? CapacityMinimum { get; set; }
            public string Content { get; set; }
            public decimal? CreditHours { get; set; }
            public string DurationUnit { get; set; }
            public int? DurationQuantity { get; set; }
            public string Instructors { get; set; }
            public DateTimeOffset? LastChangeTime { get; set; }
            public string LastChangeType { get; set; }
            public string LastChangeUser { get; set; }
            public DateTimeOffset? RegistrationDeadline { get; set; }
            public int RegistrationsCount { get; set; }
            public DateTimeOffset? RegistrationStart { get; set; }
            public Guid? VenueLocationIdentifier { get; set; }
            public string VenueLocationName { get; set; }
            public string VenueLocationOffice { get; set; }
            public string VenueRoom { get; set; }
            public bool WaitlistEnabled { get; set; }
            public string Status { get; set; }
            public string GroupPermissions { get; set; }
        }

        internal const string RegistrationStatusCountColumnPrefix = "RegistrationStatusCount:";

        public override IListSource GetExportData(QEventFilter filter, bool empty)
        {
            var result = new DataTable();

            var props = typeof(ExportDataItem).GetProperties();
            foreach (var prop in props)
            {
                var type = prop.Name == nameof(ExportDataItem.Availability)
                    ? typeof(string)
                    : Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                result.Columns.Add(prop.Name, type);
            }

            var entities = GetEvents(filter);

            foreach (var status in entities.SelectMany(x => x.Registrations).Select(x => x.ApprovalStatus).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x))
                result.Columns.Add(RegistrationStatusCountColumnPrefix + status.EmptyIfNull(), typeof(int));

            foreach (var entity in entities)
            {
                var item = GetExportDataItem(entity);
                var row = result.NewRow();

                foreach (var prop in props)
                {
                    row[prop.Name] = prop.Name == nameof(ExportDataItem.Availability)
                        ? item.Availability.ToString()
                        : prop.GetValue(item, null) ?? DBNull.Value;
                }

                foreach (var group in entity.Registrations.GroupBy(x => x.ApprovalStatus, StringComparer.OrdinalIgnoreCase))
                    row[RegistrationStatusCountColumnPrefix + group.Key.EmptyIfNull()] = group.Count();

                result.Rows.Add(row);
            }

            return result;
        }

        private static ExportDataItem GetExportDataItem(QEvent entity)
        {
            var result = new ExportDataItem();

            entity.ShallowCopyTo(result);

            if (entity.VenueLocation != null)
            {
                result.VenueLocationName = entity.VenueLocation.GroupName;

                var address = ServiceLocator.GroupSearch.GetAddress(entity.VenueLocation.GroupIdentifier, AddressType.Physical);
                if (address != null)
                    result.VenueLocationOffice = LocationHelper.ToString(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, address.Country, null, null);
            }

            result.Instructors = GetInstructors(entity);
            result.AchievementTitle = entity.Achievement?.AchievementTitle;
            result.RegistrationsCount = entity.Registrations.Count;
            result.Status = GetStatusText(entity);
            result.GroupPermissions = GetGroupPermissions(entity);

            return result;
        }

        #endregion

        #region Methods (render helpers)

        protected string GetLocalTime(string name)
        {
            var dataItem = (QEvent)Page.GetDataItem();
            var when = (DateTimeOffset?)DataBinder.Eval(dataItem, name);
            return when.Format(User.TimeZone, isHtml: true, nullValue: string.Empty);
        }

        protected string GetCapacityHtml()
        {
            var @event = (QEvent)Page.GetDataItem();
            string label;

            label = "No limitation";

            if ((@event.CapacityMinimum.HasValue) && (@event.CapacityMaximum.HasValue))
                label = $"{@event.CapacityMinimum} - {@event.CapacityMaximum}";
            else if (@event.CapacityMinimum.HasValue)
                label = $"from {@event.CapacityMinimum}";
            else if (@event.CapacityMaximum.HasValue)
                label = $"up to {@event.CapacityMaximum}";

            return label;
        }

        protected string GetRegistrationsHtml()
        {
            var @event = (QEvent)Page.GetDataItem();

            string badgeColor, badgeText;
            switch (@event.Availability)
            {
                case EventAvailabilityType.Under:
                    badgeColor = "warning";
                    badgeText = "Under";
                    break;
                case EventAvailabilityType.Over:
                    badgeColor = "danger";
                    badgeText = "Over";
                    break;
                case EventAvailabilityType.Full:
                    badgeColor = "info";
                    badgeText = "Full";
                    break;
                case EventAvailabilityType.Empty:
                    badgeColor = "warning";
                    badgeText = "Empty";
                    break;
                default:
                    badgeColor = "success";
                    badgeText = "Open";
                    break;
            }

            var html = $"<div class='clearfix'><div class='float-start'><span class='badge bg-{badgeColor}'>{badgeText}</span></div> {@event.ConfirmedRegisteredCount:n0}</div>";

            if(@event.RegistrationLocked.HasValue && @event.Availability == EventAvailabilityType.Full)
                html = $"<div class='clearfix'>{@event.ConfirmedRegisteredCount:n0}</div>";

            var waitlistedCount = @event.WaitlistedRegistrationCount;
            if (waitlistedCount > 0)
                html += $"<div class='mt-1 clearfix'><div class='float-start'><span class='badge bg-success'>Waitlisted</span></div> {waitlistedCount:n0}</div>";

            var invitedCount = @event.InvitedRegistrationCount;
            if (invitedCount > 0)
                html += $"<div class='mt-1 clearfix'><div class='float-start'><span class='badge bg-warning'>Invited</span></div> {invitedCount:n0}</div>";

            return html;
        }

        protected string GetVenueAddress()
        {
            var @event = (QEvent)Page.GetDataItem();

            if (@event.VenueLocationIdentifier == null)
                return null;

            var address = ClassVenueAddressInfo.GetAddress(@event.VenueLocationIdentifier.Value, AddressType.Physical);
            if (address == null)
                return null;

            if (!string.IsNullOrEmpty(@event.VenueRoom))
                address += ", " + @event.VenueRoom;

            return address;
        }

        protected string GetDurationHtml()
        {
            var @event = (QEvent)Page.GetDataItem();
            if (@event.DurationQuantity.HasValue && @event.DurationUnit.HasValue())
                return @event.DurationUnit.ToQuantity(@event.DurationQuantity.Value);
            return string.Empty;
        }

        protected string GetStatus()
        {
            var @event = (QEvent)Page.GetDataItem();
            var status = @event.GetClassStatus();

            return $"<span class='badge bg-{status.GetContextualClass()}'>{status.GetDescription()}</span>";
        }

        protected string GetGroupPermissions()
        {
            return GetGroupPermissions((QEvent)Page.GetDataItem());
        }

        private static string GetGroupPermissions(QEvent @event)
        {
            var groups = @event.EventGroupPermissions
                ?.Where(x => x.Group != null)
                ?.Select(x => x.Group.GroupName)
                ?.OrderBy(x => x)
                ?.ToList();

            return groups != null && groups.Count > 0
                ? string.Join(", ", groups)
                : "(public to all)";
        }

        protected string GetInstructors()
        {
            return GetInstructors((QEvent)Page.GetDataItem());
        }

        private static string GetInstructors(QEvent @event)
        {
            var instructors = @event.Attendees
                ?.Where(x => !string.IsNullOrEmpty(x.UserFullName) && x.AttendeeRole == "Instructor")
                ?.Select(x => x.UserFullName)
                ?.OrderBy(x => x)
                ?.ToList();

            return instructors != null && instructors.Count > 0
                ? string.Join(", ", instructors)
                : null;
        }

        private static string GetStatusText(QEvent @event)
        {
            return @event.GetClassStatus().GetDescription();
        }

        #endregion
    }
}