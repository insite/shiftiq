using System;
using System.Linq;

using InSite.Application.Attempts.Read;
using InSite.Application.Attempts.Write;
using InSite.Application.Banks.Write;
using InSite.Application.Cases.Write;
using InSite.Application.Contacts.Read;
using InSite.Application.Credentials.Write;
using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Issues.Read;
using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Application.Records.Read;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Application.Responses.Write;
using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Contacts.People.Forms
{
    public partial class Dashboard : AdminBasePage, IHasParentLinkParameters
    {
        private QPerson _person = null;
        private bool _isPersonLoaded = false;

        private QPerson Person
        {
            get
            {
                if (!_isPersonLoaded)
                {
                    _person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier.Value, Organization.Key, x => x.User);
                    _isPersonLoaded = true;
                }

                return _person;
            }
        }

        private Guid? UserIdentifier
            => Guid.TryParse(Request["user"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Person == null || Person.User.AccountCloaked.HasValue && !User.IsCloaked)
                RedirectToSearch();

            BindModelToControls();

            PersonDetail.BindPerson(Person, User.TimeZone);

            CancelButton.NavigateUrl = CloseButton.NavigateUrl = $"/ui/admin/contacts/people/edit?contact={UserIdentifier}";

            PageHelper.AutoBindHeader(this, null, Person.User.FullName);

            IsValid();
        }

        protected void BindModelToControls()
        {
            // Roles
            var rolesCountcount = MembershipSearch.Count(x => x.UserIdentifier == UserIdentifier && x.Group.OrganizationIdentifier == Organization.OrganizationIdentifier);
            RolesCount.Text = rolesCountcount.ToString("n0");

            var reasonsCount = ServiceLocator.MembershipReasonSearch.Count(new QMembershipReasonFilter { UserIdentifier = UserIdentifier, GroupOrganizationIdentifiers = new[] { Organization.OrganizationIdentifier } });
            MemberhipReasonCount.Text = reasonsCount.ToString("n0");

            // Validations
            var validationsCount = StandardValidationSearch.Count(x => x.UserIdentifier == UserIdentifier && x.Standard.OrganizationIdentifier == Organization.OrganizationIdentifier);
            ValidationsCount.Text = validationsCount.ToString("n0");

            // Profiles
            var profilesCount = DepartmentProfileUserSearch.Count(x => x.UserIdentifier == UserIdentifier
                && (x.Department.OrganizationIdentifier == Organization.OrganizationIdentifier || x.Profile.OrganizationIdentifier == Organization.OrganizationIdentifier));
            ProfilesCount.Text = profilesCount.ToString("n0");

            // Surveys
            var filter = new QResponseSessionFilter();
            filter.RespondentUserIdentifier = UserIdentifier;
            filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            var surveysResponsesCount = ServiceLocator.SurveySearch.CountResponseSessions(filter);
            SurveysResponsesCount.Text = surveysResponsesCount.ToString("n0");

            // Assessments
            var attemptsCount = ServiceLocator.AttemptSearch.CountAttempts(new QAttemptFilter { FormOrganizationIdentifier = Organization.OrganizationIdentifier, LearnerUserIdentifier = UserIdentifier });
            AttemptsCount.Text = attemptsCount.ToString();

            // Events
            var eventAttendiesCount = ServiceLocator.EventSearch.CountAttendees(new QEventAttendeeFilter { OrganizationIdentifier = Organization.OrganizationIdentifier, ContactIdentifier = UserIdentifier });
            EventAttendiesCount.Text = eventAttendiesCount.ToString();
            var eventRegistrationsCount = ServiceLocator.RegistrationSearch.CountRegistrations(new QRegistrationFilter { OrganizationIdentifier = Organization.OrganizationIdentifier, CandidateIdentifier = UserIdentifier });
            EventRegistrationsCount.Text = eventRegistrationsCount.ToString("n0");

            // Messages
            var messageSubscribers = ServiceLocator.MessageSearch.CountSubscriberUsers(new QSubscriberUserFilter { MessageOrganizationIdentifier = Organization.OrganizationIdentifier, SubscriberIdentifier = UserIdentifier });
            MessageSubscribers.Text = messageSubscribers.ToString("n0");

            // Issues
            var issueResponsibilities = ServiceLocator.IssueSearch.CountUsers(new QIssueUserFilter { IssueOrganizationIdentifier = Organization.OrganizationIdentifier, UserIdentifier = UserIdentifier });
            IssueResponsibilities.Text = issueResponsibilities.ToString("n0");

            // Gradebooks
            var scores = ServiceLocator.RecordSearch.GetGradebookScores(new QProgressFilter { StudentUserIdentifier = UserIdentifier }, null, null, x => x.Gradebook);
            var gradebookIdentifiers = scores.Select(x => x.GradebookIdentifier).Distinct().ToList();
            var gradebookStudentCount = gradebookIdentifiers.Count;
            GradebookStudentCount.Text = gradebookStudentCount.ToString("n0");

            // Achievements
            var credentialsCount = ServiceLocator.AchievementSearch.CountCredentials(new VCredentialFilter { OrganizationIdentifier = Organization.OrganizationIdentifier, UserIdentifier = UserIdentifier });
            CredentialsCount.Text = credentialsCount.ToString("n0");

            // Comments
            var commentsCount = QCommentSearch.Count(x => x.TopicUserIdentifier == UserIdentifier);
            CommentsCount.Text = commentsCount.ToString("n0");

            // Experiences
            var experiencesCount = VCmdsCredentialAndExperienceSearch.Count(x => x.UserIdentifier == UserIdentifier);
            ExperiencesCount.Text = experiencesCount.ToString("n0");
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (Person == null)
            {
                RedirectToSearch();
                return;
            }

            Delete(Person.OrganizationIdentifier, Person.UserIdentifier);

            RedirectToSearch();
        }

        public static void Delete(Guid organizationIdentifier, Guid userIdentifier)
        {
            var attempts = ServiceLocator.AttemptSearch.GetAttempts(new QAttemptFilter
            {
                FormOrganizationIdentifier = Organization.OrganizationIdentifier,
                LearnerUserIdentifier = userIdentifier
            });

            var attendees = ServiceLocator.EventSearch.GetAttendees(new QEventAttendeeFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                ContactIdentifier = userIdentifier
            });

            var credentials = ServiceLocator.AchievementSearch.GetCredentials(new VCredentialFilter()
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                UserIdentifier = userIdentifier
            });

            var gradebooks = ServiceLocator.RecordSearch.GetGradebooks(new QGradebookFilter()
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                StudentIdentifier = userIdentifier
            });

            var registrationIds = ServiceLocator.RegistrationSearch.GetRegistrationIdentifiers(new QRegistrationFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                CandidateIdentifier = userIdentifier
            });

            var subscribers = ServiceLocator.MessageSearch.GetSubscriberUsers(new QSubscriberUserFilter
            {
                MessageOrganizationIdentifier = Organization.OrganizationIdentifier,
                SubscriberIdentifier = userIdentifier
            });

            var users = ServiceLocator.IssueSearch.GetUsers(new QIssueUserFilter
            {
                IssueOrganizationIdentifier = Organization.OrganizationIdentifier,
                UserIdentifier = userIdentifier
            });

            var surveyResponses = ServiceLocator.SurveySearch.GetResponseSessions(new QResponseSessionFilter
            {
                RespondentUserIdentifier = userIdentifier,
                OrganizationIdentifier = Organization.OrganizationIdentifier
            });

            foreach (var attempt in attempts)
                ServiceLocator.SendCommand(new VoidAttempt(attempt.AttemptIdentifier, "Person Deleted"));

            var forms = ServiceLocator.BankSearch.GetForms(attempts.Select(x => x.FormIdentifier).Distinct());
            foreach (var form in forms)
                ServiceLocator.SendCommand(new AnalyzeForm(form.BankIdentifier, form.FormIdentifier));

            foreach (var attendee in attendees)
                ServiceLocator.SendCommand(new RemoveEventAttendee(attendee.EventIdentifier, attendee.UserIdentifier));

            foreach (var credential in credentials)
                ServiceLocator.SendCommand(new DeleteCredential(credential.CredentialIdentifier));

            foreach (var gradebook in gradebooks)
            {
                var isLocked = gradebook.IsLocked;

                if (isLocked)
                    ServiceLocator.SendCommand(new UnlockGradebook(gradebook.GradebookIdentifier));

                ServiceLocator.SendCommand(new DeleteEnrollment(gradebook.GradebookIdentifier, userIdentifier));

                if (isLocked)
                    ServiceLocator.SendCommand(new LockGradebook(gradebook.GradebookIdentifier));
            }

            foreach (var registrationId in registrationIds)
                ServiceLocator.SendCommand(new DeleteRegistration(registrationId, false));

            foreach (var subscriber in subscribers)
                ServiceLocator.SendCommand(new RemoveMessageSubscriber(subscriber.MessageIdentifier, subscriber.UserIdentifier, false));

            foreach (var user in users)
                ServiceLocator.SendCommand(new UnassignUser(user.IssueIdentifier, user.UserIdentifier, user.IssueRole));

            foreach (var response in surveyResponses)
                ServiceLocator.SendCommand(new DeleteResponseSession(response.ResponseSessionIdentifier));

            PersonStore.Delete(userIdentifier, organizationIdentifier);
        }

        new private bool IsValid()
        {
            if (Person == null)
                return ShowError("User not found.");

            var messageCount = ServiceLocator.MessageSearch.CountMessages(new MessageFilter { SenderIdentifier = Person.User.UserIdentifier });
            if (messageCount > 0)
                return ShowError($"This contact person is the sender for {messageCount:n0} messages, therefore cannot be deleted.");

            return true;

            bool ShowError(string html)
            {
                ErrorText.Text = html;
                ErrorPanel.Visible = CloseButton.Visible = true;
                DeleteButton.Visible = CancelButton.Visible = ConfirmMessage.Visible = false;
                return false;
            }
        }

        private void RedirectToSearch()
            => HttpResponseHelper.Redirect("/ui/admin/contacts/people/search");

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/edit") ? $"contact={UserIdentifier}" : null;
    }
}