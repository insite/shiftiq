using System;
using System.Linq;

using InSite.Application.Attempts.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Courses.Write;
using InSite.Application.Credentials.Write;
using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Application.Records.Write;
using InSite.Domain.Messages;
using InSite.Domain.Records;

using Shift.Common.Timeline.Changes;

namespace InSite.Application.Records.Read
{
    /// <summary>
    /// Implements the process manager for Achievement changes. 
    /// </summary>
    /// <remarks>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain changes in a cross-aggregate, 
    /// eventually consistent manner. Time can be a trigger. Process managers are sometimes purely reactive, and sometimes represent workflows. From 
    /// an implementation perspective, a process manager is a state machine that is driven forward by incoming changes (which may come from many 
    /// aggregates). Some states will have side effects, such as sending commands, talking to external web services, or sending emails.
    /// </remarks>
    public class AchievementChangeProcessor
    {
        private readonly ICommander _commander;
        private readonly IAlertMailer _mailer;

        private readonly IAchievementSearch _achievements;
        private readonly ICourseObjectSearch _courses;
        private readonly IAttemptSearch _attempts;
        private readonly IJournalSearch _journals;
        private readonly IProgramSearch _program;
        private readonly IMessageSearch _messages;
        private readonly IContentSearch _contents;
        private readonly IContactSearch _contacts;
        private readonly IProgramStore _programStore;
        private readonly IProgramService _programService;

        private readonly bool _restartCourseWhenCredentialExpired;

        public AchievementChangeProcessor(
            ICommander commander,
            IChangeQueue publisher,
            IAlertMailer mailer,
            IAchievementSearch achievements,
            ICourseObjectSearch courses,
            IAttemptSearch attempts,
            IJournalSearch journals,
            IProgramSearch program,
            IProgramStore programStore,
            IProgramService programService,
            IMessageSearch messages,
            IContentSearch contents,
            IContactSearch contacts,
            bool restartCourseWhenCredentialExpired)
        {
            _commander = commander;
            _mailer = mailer;

            _achievements = achievements;
            _courses = courses;
            _attempts = attempts;
            _journals = journals;
            _program = program;
            _messages = messages;
            _contents = contents;
            _contacts = contacts;
            _programStore = programStore;
            _programService = programService;

            _restartCourseWhenCredentialExpired = restartCourseWhenCredentialExpired;

            publisher.Subscribe<AchievementDeleted>(Handle);
            publisher.Subscribe<CredentialCreated>(Handle);
            publisher.Subscribe<CredentialExpired2>(Handle);
            publisher.Subscribe<CredentialGranted3>(Handle);
        }

        public void Handle(AchievementDeleted c)
        {
            if (!c.Cascade)
                return;

            var filter = new VCredentialFilter
            {
                AchievementIdentifier = c.AggregateIdentifier
            };
            var credentials = _achievements.GetCredentials(filter);
            foreach (var credential in credentials)
                _commander.Send(new DeleteCredential(credential.CredentialIdentifier));
        }

        public void Handle(CredentialCreated e)
        {
            var credential = _achievements.GetCredential(e.AggregateIdentifier);
            if (credential == null)
                return;

            var notification = new CredentialCreatedNotification
            {
                OriginOrganization = e.OriginOrganization,
                OriginUser = e.OriginUser,

                AchievementType = credential.AchievementLabel,
                AchievementName = credential.AchievementTitle,

                LearnerIdentifier = credential.UserIdentifier,
                LearnerFirstName = credential.UserFirstName,
                LearnerLastName = credential.UserLastName,
                LearnerEmail = credential.UserEmail
            };

            _mailer.Send(notification, credential.UserIdentifier);
        }

        /// <summary>
        /// After a credential is expired, find any e-learning modules that are used to grant it, and restart those
        /// modules (i.e. remove course progression records and void assessment attempts).
        /// </summary>
        public void Handle(CredentialExpired2 e)
        {
            if (!_restartCourseWhenCredentialExpired)
                return;

            var credential = _achievements.GetCredential(e.AggregateIdentifier);
            if (credential == null)
                return;

            var enrollments = _achievements.SelectGradebookEnrollmentsForCredentials(
                credential.UserIdentifier,
                credential.AchievementIdentifier,
                pending: true,
                valid: true,
                expired: true);

            foreach (var enrollment in enrollments)
                _commander.Send(new RestartCourseEnrollment(enrollment.LearnerIdentifier, enrollment.CourseIdentifier));
        }

        /// <summary>
        /// If the achievement has a valid webhook, then build and send an HTTP POST Request to the endpoint for this learner/achievement completion.
        /// </summary>
        public void Handle(CredentialGranted3 x)
        {
            var credential = _achievements.GetCredential(x.AggregateIdentifier);
            if (credential == null)
                return;

            var journal = _journals.GetJournalSetups(new QJournalSetupFilter()
            {
                AchievementIdentifier = credential.AchievementIdentifier,
                OrganizationIdentifier = credential.OrganizationIdentifier
            }, null, null).FirstOrDefault();

            if (journal != null)
            {
                CompleteUserProgramTaskEnrollment(journal, credential);
                SendProgramCompletionNotificationMessage(journal, credential);
            }
            else
                CompleteUserProgramTaskEnrollment(credential.UserIdentifier, credential.AchievementIdentifier, credential.OrganizationIdentifier);
        }

        private void CompleteUserProgramTaskEnrollment(QJournalSetup journal, VCredential credential)
        {
            CompleteUserProgramTaskEnrollment(credential.UserIdentifier, journal.JournalSetupIdentifier, journal.OrganizationIdentifier);
        }

        private void CompleteUserProgramTaskEnrollment(Guid UserIdentifier, Guid ObjectIdentifier, Guid OrganizationIdentifier)
        {
            var enrollmentsToCheck = _programStore.TaskCompleted(UserIdentifier, OrganizationIdentifier, ObjectIdentifier);
            foreach (var enrollment in enrollmentsToCheck)
                _programService.CompletionOfProgramAchievement(enrollment.ProgramIdentifier, enrollment.LearnerIdentifier, enrollment.OrganizationIdentifier);
        }

        private void SendProgramCompletionNotificationMessage(QJournalSetup journal, VCredential credential)
        {
            var commands = ProgramEnrollment.ProgramEnrollmentCompletion(journal.JournalSetupIdentifier, credential.UserIdentifier, journal.OrganizationIdentifier
                , _mailer, _messages, _contents, _contacts, _program, _programStore, _achievements);

            commands.AddRange(ProgramEnrollment.ProgramEnrollmentForStandAloneAchievementCompletion(journal.JournalSetupIdentifier, credential.UserIdentifier, journal.OrganizationIdentifier
                , _mailer, _messages, _contents, _contacts, _program, _programStore, _achievements));

            if (commands != null)
                foreach (var command in commands.ToArray())
                    _commander.Send(command);
        }
    }
}
