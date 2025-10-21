using System;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Registrations.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Events.Registrations.Exams.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QRegistrationFilter>
    {
        protected bool CanWrite { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var registrationIdentifier = (Guid)DataBinder.Eval(e.Row.DataItem, "RegistrationIdentifier");

            var registrationLink = (IconLink)e.Row.FindControl("RegistrationLink");
            registrationLink.NavigateUrl = new ReturnUrl(null).GetRedirectUrl($"/ui/admin/registrations/exams/edit?registration={registrationIdentifier}");
        }

        protected override int SelectCount(QRegistrationFilter filter)
        {
            return ServiceLocator.RegistrationSearch.CountRegistrations(filter);
        }

        protected override IListSource SelectData(QRegistrationFilter filter)
        {
            CanWrite = Identity.IsGranted(Route.ToolkitNumber, PermissionOperation.Write);

            filter.OrderBy = "RegistrationRequestedOn desc";

            return ServiceLocator.RegistrationSearch
                .GetRegistrations(filter, x => x.Candidate, x => x.Event.VenueOffice, x => x.Event.VenueLocation, x => x.Form)
                .Select(x => new
                {
                    EventIdentifier = x.EventIdentifier,

                    EventType = x.Event.EventType,
                    EventTypePlural = x.Event.EventType.Pluralize().ToLower(),
                    
                    EventScheduledStart = x.Event.EventScheduledStart,
                    EventNumber = x.Event.EventNumber,
                    EventTitle = x.Event.EventTitle,
                    EventVenueOffice = x.Event.VenueOfficeName,
                    EventVenueLocation = x.Event.VenueLocationName,
                    EventVenueRoom = x.Event.VenueRoom,

                    AssessmentFormCode = x.Form?.FormCode,
                    AssessmentFormName = x.Form?.FormName,

                    LearnerIdentifier = x.CandidateIdentifier,
                    LearnerName = x.Candidate?.UserFullName,
                    LearnerCode = x.Candidate?.PersonCode,
                    LearnerEmail = x.Candidate?.UserEmail,
                    LearnerEmailEnabled = x.Candidate?.UserEmailEnabled ?? false,

                    RegistrationIdentifier = x.RegistrationIdentifier,
                    RegistrationRequestedOn = x.RegistrationRequestedOn,
                    RegistrationType = x.CandidateType,
                    ApprovalStatus = x.ApprovalStatus,
                    AttendanceStatus = x.AttendanceStatus,
                    RegistrationSequence = x.RegistrationSequence,
                    RegistrationComment = x.RegistrationComment,
                })
                .ToList()
                .ToSearchResult();
        }

        protected static string GetLocalTime(object item)
        {
            if (item == null)
                return null;

            var when = (DateTimeOffset)item;
            var date = when.FormatDateOnly(User.TimeZone);
            var time = when.FormatTimeOnly(User.TimeZone);

            return $"{date}<br/><span class='form-text'>{time}</span>";
        }

        protected string GetFormName(string forms)
        {
            return forms.IsEmpty()
                ? forms
                : string.Concat(forms.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => $"<p>{HttpUtility.HtmlEncode(x)}</p>"));
        }
    }
}