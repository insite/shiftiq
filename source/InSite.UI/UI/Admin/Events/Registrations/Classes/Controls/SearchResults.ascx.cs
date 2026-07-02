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
            CanWrite = Identity.IsGranted(Route.ToolkitNumber, DataAccess.Update);

            filter.OrderBy = "RegistrationRequestedOn desc";

            return ServiceLocator.RegistrationSearch.GetRegistrationSearchResults(filter)
                .Select(x => new
                {
                    EventScheduledStart = x.EventScheduledStart,
                    EventScheduledEnd = x.EventScheduledEnd,
                    EventType = x.EventType,
                    EventTypePlural = x.EventType.Pluralize().ToLower(),
                    EventIdentifier = x.EventIdentifier,
                    EventTitle = x.EventTitle,
                    EventAchievementTitle = x.EventAchievementTitle,
                    EventAchievementDescription = x.EventAchievementDescription,
                    RegistrationRequestedOn = x.RegistrationRequestedOn,
                    CandidateIdentifier = x.CandidateIdentifier,
                    UserFullName = x.CandidateFullName,
                    PersonCode = x.CandidatePersonCode,
                    RegistrantIsELL = x.CandidateFirstLanguage,
                    RegistrantPostalCode = x.CandidatePostalCode,
                    ApprovalStatus = x.ApprovalStatus,
                    AttendanceStatus = x.AttendanceStatus,
                    RegistrationFee = x.RegistrationFee,
                    RegistrationIdentifier = x.RegistrationIdentifier,
                    Email = x.CandidateEmail,
                    EmailEnabled = x.CandidateEmailEnabled,
                    EmployerGroupName = x.EmployerGroupName,
                    EmployerGroupIdentifier = x.EmployerGroupIdentifier,
                    EmployerGroupRegion = x.EmployerGroupRegion,
                    EmployerGroupStatus = x.EmployerGroupStatus,
                    Phone = x.CandidatePhone,
                    LearnerId = x.CandidatePersonCode,
                    RegistrationSequence = x.RegistrationSequence,
                    WorkBasedHoursToDate = x.WorkBasedHoursToDate,
                    RegistrationComment = x.RegistrationComment,
                    IncludeInT2202 = x.IncludeInT2202 ? "Yes" : "No",
                    PaymentStatus = string.Equals(x.PaymentStatus, "Completed", StringComparison.OrdinalIgnoreCase) ? "Paid" : x.PaymentStatus,
                    RegistrationRequestedByIdentifier = x.RegistrationRequestedByIdentifier,
                    RegistrationRequestedByName = x.RegistrationRequestedByName,
                    RegistrationRequestedByEmail = x.RegistrationRequestedByEmail,
                    BillingCode = x.BillingCode,
                    Department = string.Join(", ", x.DepartmentNames),
                    ExamFormTitle = x.ExamFormTitle,
                    ExamFormName = x.ExamFormName,
                    ExamFormCode = x.ExamFormCode
                })
                .ToList()
                .ToSearchResult();
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
