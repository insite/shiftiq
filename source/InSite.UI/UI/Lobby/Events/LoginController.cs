using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.People.Write;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Portal.Assessments.Attempts.Utilities;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.Contract.Booking;

namespace InSite.UI.Lobby
{
    public class LoginController
    {
        private readonly OrganizationState _organization;
        private readonly Literal _errorMessage;

        public LoginController(OrganizationState organization, Literal errorMessage)
        {
            _organization = organization;
            _errorMessage = errorMessage;
        }

        public void Submit(string personCode, string passcode)
        {
            var person = GetPerson(personCode);

            var registration = ValidateAndGetRegistration(personCode, passcode, person, _organization);
            if (registration == null)
                return;

            AutoAuthenticateUser(personCode, person, registration);

            var url = AttemptHelper.GetRegistrationStartUrl(registration.RegistrationIdentifier);

            HttpResponseHelper.Redirect(url, true);
        }

        private Person GetPerson(string personCode)
        {
            // First look for a contact person where the User Name matches the Candidate Code.
            // If not found then look for person where the Account Number matches the Candidate Code.

            return PersonCriteria.SelectFirst(new PersonFilter { OrganizationIdentifier = _organization.Identifier, EmailExact = personCode }, x => x.User)
                ?? PersonCriteria.SelectFirst(new PersonFilter { OrganizationIdentifier = _organization.Identifier, CodeExact = personCode }, x => x.User);
        }

        private void AutoAuthenticateUser(string personCode, Person person, QRegistration registration)
        {
            // A contact person must exist.
            var p = PersonSearch.Select(person.OrganizationIdentifier, person.UserIdentifier);
            if (p == null)
                return;

            // If the person is not granted access then automatically grant access.
            if (!p.UserAccessGranted.HasValue)
                ServiceLocator.SendCommand(new GrantPersonAccess(p.PersonIdentifier, DateTimeOffset.UtcNow, person.User.FullName));

            // If the event is not a class then count this person as an attendee. (Does anyone know why we don't count
            // this for class events?)
            if (!StringHelper.Equals(registration.Event.EventType, "Class"))
                ServiceLocator.SendCommand(new TakeAttendance(registration.RegistrationIdentifier, "Present", null, null));

            // Authenticate the user's session.

            CurrentSessionState.DateSignedIn = DateTime.UtcNow;

            CurrentIdentityFactory.SignedIn(
                personCode,
                person.UserIdentifier,
                _organization.OrganizationCode,
                null,
                null,
                person.Language.NullIfEmpty() ?? CookieTokenModule.Current.Language.NullIfEmpty() ?? Language.Default,
                person.User.TimeZone,
                AuthenticationSource.ShiftIqExamEvent);

            SessionHelper.StartSession(_organization.OrganizationIdentifier, person.UserIdentifier);

            AccountHelper.WriteLoginEvent(AuthenticationResult.Success, person.User, _organization.Identifier, AuthenticationSource.ShiftIqExamEvent);
        }

        private QRegistration ValidateAndGetRegistration(string learner, string passcode, Person person, OrganizationState organization)
        {
            string error = null;

            if (person == null)
            {
                error = $"The exam candidate code <strong>{learner}</strong> does not match any registered user accounts.";
                SetErrorMessage("Access Denied", error);
                return null;
            }

            var registrations = ServiceLocator.RegistrationSearch
                .GetRegistrationsByCandidate(person.UserIdentifier,
                    x => x.Event,
                    x => x.Event.GradebookEvents.Select(y => y.Gradebook), // Needed for GetClassStatus()
                    x => x.Form,
                    x => x.Accommodations
                )
                .Where(x => x.RegistrationPassword == passcode)
                .ToList();

            QRegistration registration = null;

            if (registrations.Count == 0)
                error = $"The exam event password <strong>{passcode}</strong> does not match the registration for your current exam.";
            else if (registrations.Count > 1)
                error = $"The exam event password <strong>{passcode}</strong> matches multiple registrations for your current exam.";
            else
            {
                registration = registrations[0];

                var validationModel = GetValidationModel(person, registration, organization);

                error = StringHelper.Equals(registration.Event.EventType, "Class")
                    ? new ClassEventValidator().Validate(validationModel)
                    : new ExamEventValidator().Validate(validationModel);
            }

            if (!string.IsNullOrEmpty(error))
            {
                SetErrorMessage("Access Denied", error, person.User);
                return null;
            }

            return registration;
        }

        private EventValidationModel GetValidationModel(Person person, QRegistration registration, OrganizationState organization)
        {
            var model = new EventValidationModel
            {
                ApprovalStatus = registration.ApprovalStatus,
                AllowLoginAnyTime = organization.Toolkits?.Events?.AllowLoginAnyTime ?? false,
                HasEvent = registration.Event != null,
                EventFormat = registration.Event?.EventFormat,
                EventSchedulingStatus = registration.Event?.EventSchedulingStatus,
                EventScheduledStart = registration.Event?.EventScheduledStart ?? DateTimeOffset.UtcNow,
                ExamFormIdentifier = registration.ExamFormIdentifier,
                HasForm = registration.Form != null,
                AssessmentCount = registration.Form != null
                    ? ServiceLocator.PageSearch.Count(x => x.ObjectType == "Assessment" && x.ObjectIdentifier == registration.ExamFormIdentifier.Value)
                    : 0,
                HasAccommodations = registration.Accommodations != null,
                HasResumeInterruptedOnlineExam = registration.Accommodations != null
                    && registration.Accommodations.Any(x => x.AccommodationType == "Resume Interrupted Online Exam"),

                UserTimeZone = person.User.TimeZone,

                ClassStatus = registration.Event?.GetClassStatus()
            };

            return model;
        }

        private void SetErrorMessage(string title, string text, User user = null)
        {
            _errorMessage.Text = $@"
<div class='alert alert-danger' style='margin-bottom: 20px;'>
    <div style='font-weight:bold;'>
        <i class='fas fa-stop-circle'></i>
        {title}
    </div>
    {text}
</div>";

            if (user != null)
                AccountHelper.WriteLoginEvent(AuthenticationResult.AccessDenied, user, _organization.Identifier, AuthenticationSource.ShiftIqExamEvent, text);
        }
    }
}