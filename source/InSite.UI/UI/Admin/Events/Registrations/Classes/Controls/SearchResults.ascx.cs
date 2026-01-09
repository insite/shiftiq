using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Registrations.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Events.Registrations.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QRegistrationFilter>
    {
        protected bool CanWrite { get; set; }

        protected override int SelectCount(QRegistrationFilter filter)
        {
            return ServiceLocator.RegistrationSearch.CountRegistrations(filter);
        }

        protected override IListSource SelectData(QRegistrationFilter filter)
        {
            CanWrite = Identity.IsGranted(Route.ToolkitNumber, PermissionOperation.Write);

            filter.OrderBy = "RegistrationRequestedOn desc";

            return ServiceLocator.RegistrationSearch
                .GetRegistrations(
                    filter,
                    x => x.Event.Achievement,
                    x => x.Candidate.HomeAddress,
                    x => x.Candidate.User.Memberships.Select(y => y.Group),
                    x => x.Employer,
                    x => x.Customer,
                    x => x.Payment,
                    x => x.RegistrationRequestedByPerson,
                    x => x.Seat
                )
                .Select(x => new
                {
                    EventScheduledStart = x.Event.EventScheduledStart,
                    EventScheduledEnd = x.Event.EventScheduledEnd,
                    EventType = x.Event.EventType,
                    EventTypePlural = x.Event.EventType.Pluralize().ToLower(),
                    EventIdentifier = x.EventIdentifier,
                    EventTitle = x.Event.EventTitle,
                    EventAchievementTitle = x.Event.Achievement?.AchievementTitle,
                    EventAchievementDescription = x.Event.Achievement?.AchievementDescription,
                    RegistrationRequestedOn = x.RegistrationRequestedOn,
                    CandidateIdentifier = x.CandidateIdentifier,
                    UserFullName = x.Candidate?.UserFullName,
                    PersonCode = x.Candidate?.PersonCode,
                    RegistrantIsELL = x.Candidate?.FirstLanguage,
                    RegistrantPostalCode = x.Candidate?.HomeAddress?.PostalCode,
                    ApprovalStatus = x.ApprovalStatus,
                    AttendanceStatus = x.AttendanceStatus,
                    RegistrationFee = x.RegistrationFee,
                    RegistrationIdentifier = x.RegistrationIdentifier,
                    Email = x.Candidate?.UserEmail,
                    EmailEnabled = x.Candidate?.UserEmailEnabled,
                    EmployerGroupName = x.Employer?.GroupName,
                    EmployerGroupIdentifier = x.Employer?.GroupIdentifier,
                    EmployerGroupRegion = x.Employer?.GroupRegion,
                    EmployerGroupStatus = x.Employer?.GroupStatus,
                    Phone = x.Candidate?.UserPhone,
                    LearnerId = x.Candidate?.PersonCode,
                    RegistrationSequence = x.RegistrationSequence,
                    WorkBasedHoursToDate = x.WorkBasedHoursToDate,
                    RegistrationComment = x.RegistrationComment,
                    IncludeInT2202 = x.IncludeInT2202 ? "Yes" : "No",
                    PaymentStatus = string.Equals(x.Payment?.PaymentStatus, "Completed", StringComparison.OrdinalIgnoreCase) ? "Paid" : x.Payment?.PaymentStatus,
                    RegistrationRequestedByIdentifier = x.RegistrationRequestedByPerson?.UserIdentifier,
                    RegistrationRequestedByName = x.RegistrationRequestedByPerson?.UserFullName,
                    RegistrationRequestedByEmail = x.RegistrationRequestedByPerson?.UserEmail,
                    BillingCode = x.BillingCode,
                    Department = GetDepartment(x)
                })
                .ToList()
                .ToSearchResult();
        }

        private static string GetDepartment(QRegistration registration)
        {
            var departments = registration.Candidate.User.Memberships
                .Where(x =>
                    x.Group.OrganizationIdentifier == Organization.Identifier
                    && string.Equals(x.Group.GroupType, GroupTypes.Department)
                )
                .Select(y => y.Group.GroupName);

            return string.Join(", ", departments);
        }

        protected string GetScheduledTime()
        {
            var item = Page.GetDataItem();
            var eventScheduledStart = (DateTimeOffset)DataBinder.Eval(item, "EventScheduledStart");
            var eventScheduledEnd = (DateTimeOffset?)DataBinder.Eval(item, "EventScheduledEnd");

            var text = TimeZones.Format(eventScheduledStart, User.TimeZone, true);
            if (eventScheduledEnd == null)
                return text;

            text += " to ";

            if (eventScheduledStart.UtcDateTime.Date == eventScheduledEnd.Value.UtcDateTime.Date)
            {
                var endTime = TimeZones.FormatTimeOnly(eventScheduledEnd.Value.UtcDateTime, User.TimeZone);
                text += $"<span class='form-text text-body-secondary'>{endTime}</span>";
            }
            else
                text += TimeZones.Format(eventScheduledEnd.Value.UtcDateTime, User.TimeZone, true);

            return text;
        }
    }
}