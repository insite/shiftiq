using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contacts.Read;
using InSite.Application.Journals.Write;
using InSite.Application.Messages.Write;
using InSite.Application.Organizations.Read;
using InSite.Domain.Messages;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Records.Read
{
    public class JournalChangeProcessor
    {
        private const int RepeatedNotificationIntervalSec = 10;

        private readonly Urls _urls;
        private readonly ICommander _commander;
        private readonly IJournalSearch _journals;
        private readonly IContactSearch _contacts;
        private readonly Dictionary<Guid, DateTime> _notifiedLogbooks;
        private readonly IAlertMailer _mailer;
        private readonly IOrganizationSearch _organizations;

        public JournalChangeProcessor(
            Urls urls,
            ICommander commander,
            IChangeQueue publisher,
            IJournalSearch journals,
            IContactSearch contacts,
            IAlertMailer mailer,
            IOrganizationSearch organizations
            )
        {
            _urls = urls;
            _commander = commander;
            _journals = journals;
            _contacts = contacts;
            _mailer = mailer;
            _organizations = organizations;

            _notifiedLogbooks = new Dictionary<Guid, DateTime>();

            publisher.Subscribe<JournalSetupUserAdded>(Handle);
            publisher.Subscribe<JournalSetupUserDeleted>(Handle);

            publisher.Subscribe<CommentAdded>(Handle);
            publisher.Subscribe<CommentChanged>(Handle);
            publisher.Subscribe<ExperienceAdded>(Handle);
            publisher.Subscribe<ExperienceCompetencyAdded>(Handle);
            publisher.Subscribe<ExperienceCompetencyChanged>(Handle);
            publisher.Subscribe<ExperienceCompetencySatisfactionLevelChanged>(Handle);
            publisher.Subscribe<ExperienceCompetencySkillRatingChanged>(Handle);
            publisher.Subscribe<ExperienceCompletedChanged>(Handle);
            publisher.Subscribe<ExperienceEmployerChanged>(Handle);
            publisher.Subscribe<ExperienceEvidenceChanged>(Handle);
            publisher.Subscribe<ExperienceHoursChanged>(Handle);
            publisher.Subscribe<ExperienceInstructorChanged>(Handle);
            publisher.Subscribe<ExperienceSupervisorChanged>(Handle);
            publisher.Subscribe<ExperienceTimeChanged>(Handle);
            publisher.Subscribe<ExperienceTrainingChanged>(Handle);
            publisher.Subscribe<ExperienceDeleted>(Handle);
            publisher.Subscribe<ExperienceCompetencyDeleted>(Handle);
            publisher.Subscribe<ExperienceValidated>(Handle);
            publisher.Subscribe<ExperienceMediaEvidenceChanged>(Handle);
            publisher.Subscribe<ExperienceCapturedEvidenceChanged>(Handle);
        }

        private void Handle(JournalSetupUserAdded change)
        {
            if (change.Role != JournalSetupUserRole.Learner)
                return;

            var notification = CreateJoinedNotification(change);
            SendNotificationForLogbookJoined(notification);
        }

        private void Handle(JournalSetupUserDeleted change)
        {
            var journals = _journals.GetJournals(new QJournalFilter { JournalSetupIdentifier = change.AggregateIdentifier, UserIdentifier = change.User });
            foreach (var journal in journals)
                _commander.Send(new DeleteJournal(journal.JournalIdentifier));
        }

        private void Handle(CommentAdded change)
        {
            if (change.IsPrivate)
                return;

            var notification = CreateChangedNotification(change, null, "Logbook Comment Posted");
            SendNotificationForLogbookChanged(notification);
        }

        private void Handle(CommentChanged change)
        {
            if (change.IsPrivate)
                return;

            var notification = CreateChangedNotification(change, null, "Logbook Comment Modified");
            SendNotificationForLogbookChanged(notification);
        }

        private void Handle(ExperienceAdded change)
        {
            OnLogbookModified(change, change.Experience, "New Entry");
        }

        private void Handle(ExperienceCompetencyAdded change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceCompetencyChanged change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceCompetencySatisfactionLevelChanged change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceCompetencySkillRatingChanged change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceCompletedChanged change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceEmployerChanged change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceEvidenceChanged change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceHoursChanged change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceInstructorChanged change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceSupervisorChanged change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceTimeChanged change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceTrainingChanged change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceMediaEvidenceChanged change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceCapturedEvidenceChanged change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceDeleted change)
        {
            OnLogbookModified(change, change.Experience, "Entry Deleted");
        }

        private void Handle(ExperienceCompetencyDeleted change)
        {
            OnLogbookModified(change, change.Experience, "Entry Updated");
        }

        private void Handle(ExperienceValidated change)
        {
            OnLogbookModified(change, change.Experience, "Entry Validated");
        }

        private void OnLogbookModified(Change change, Guid? experienceIdentifier, string modificationType)
        {
            lock (_notifiedLogbooks)
            {
                if (_notifiedLogbooks.ContainsKey(change.AggregateIdentifier))
                {
                    var lastNotified = _notifiedLogbooks[change.AggregateIdentifier];
                    var nextNotified = lastNotified.AddSeconds(RepeatedNotificationIntervalSec);

                    if (DateTime.Now < nextNotified)
                        return;
                }

                _notifiedLogbooks[change.AggregateIdentifier] = DateTime.Now;
            }

            var notification = CreateChangedNotification(change, experienceIdentifier, modificationType);
            SendNotificationForLogbookChanged(notification);
        }

        private void SendNotificationForLogbookJoined(LogbookJoinedNotification notification)
        {
            if (notification == null)
                return;

            if (notification.MessageToLearnerWhenLogbookStarted.HasValue)
            {
                notification.MessageIdentifier = notification.MessageToLearnerWhenLogbookStarted;
                _mailer.Send(notification, notification.LearnerIdentifier);
            }
        }

        private void SendNotificationForLogbookChanged(LogbookChangedNotification notification)
        {
            if (notification == null)
                return;

            if (notification.MessageToLearnerWhenLogbookModified.HasValue && notification.LearnerIdentifier != notification.OriginUser)
            {
                notification.MessageIdentifier = notification.MessageToLearnerWhenLogbookModified;
                _mailer.Send(notification, notification.LearnerIdentifier);
            }

            if (notification.MessageToValidatorWhenLogbookModified.HasValue)
            {
                var validatorIds = notification.ValidatorIdentifiers
                    .EmptyIfNull().Where(x => x != notification.OriginUser).ToArray();
                if (validatorIds.IsNotEmpty())
                {

                    notification.LogbookUrl = notification.ExperienceIdentifier.HasValue
                        ? $"{notification.AppUrl}/ui/admin/records/logbooks/validators/entries/view-experience?experience={notification.ExperienceIdentifier}"
                        : $"{notification.AppUrl}/ui/admin/records/logbooks/validators/outline-journal?journalsetup={notification.JournalSetupIdentifier}&user={notification.LearnerIdentifier}";

                    notification.MessageIdentifier = notification.MessageToValidatorWhenLogbookModified;

                    foreach (var validatorId in validatorIds)
                        _mailer.Send(notification, validatorId);
                }
            }
        }

        private LogbookJoinedNotification CreateJoinedNotification(JournalSetupUserAdded change)
        {
            var variables = new LogbookJoinedNotification();
            var journalSetup = _journals.GetJournalSetup(change.AggregateIdentifier);

            InitLogbookNotification(change, variables, journalSetup);

            variables.LearnerIdentifier = change.User;

            return variables;
        }

        private LogbookChangedNotification CreateChangedNotification(Change change, Guid? experienceIdentifier, string modificationType)
        {
            var variables = new LogbookChangedNotification();
            var journal = _journals.GetJournal(change.AggregateIdentifier, x => x.JournalSetup);

            InitLogbookNotification(change, variables, journal.JournalSetup);

            variables.ModificationType = modificationType;
            variables.ExperienceIdentifier = experienceIdentifier;

            if (journal != null)
            {
                variables.LearnerIdentifier = journal.UserIdentifier;
                variables.ValidatorIdentifiers = GetValidators(journal.JournalSetupIdentifier, change.OriginUser);
            }

            return variables;
        }

        private void InitLogbookNotification(Change change, ILogbookNotification variables, QJournalSetup journalSetup)
        {
            variables.OriginOrganization = change.OriginOrganization;
            variables.OriginUser = change.OriginUser;

            var organization = _organizations.Get(change.OriginOrganization);
            variables.OrganizationCode = organization.OrganizationCode;
            variables.OrganizationName = organization.CompanyName;
            variables.AppUrl = _urls.GetApplicationUrl(variables.OrganizationCode);

            if (journalSetup != null)
            {
                variables.JournalSetupIdentifier = journalSetup.JournalSetupIdentifier;

                variables.LogbookTitle = journalSetup.JournalSetupName;
                variables.LogbookUrl = $"{variables.AppUrl}/ui/portal/records/logbooks/outline?journalsetup={journalSetup.JournalSetupIdentifier}";

                variables.MessageToLearnerWhenLogbookModified = journalSetup.LearnerMessageIdentifier;
                variables.MessageToLearnerWhenLogbookStarted = journalSetup.LearnerAddedMessageIdentifier;
                variables.MessageToValidatorWhenLogbookModified = journalSetup.ValidatorMessageIdentifier;
            }

            var changeAuthor = _contacts.GetUser(change.OriginUser);
            variables.ActorName = changeAuthor?.UserFullName ?? "Unknown";
        }

        private Guid[] GetValidators(Guid setup, Guid excludeValidator)
        {
            var filter = new VJournalSetupUserFilter { JournalSetupIdentifier = setup, Role = JournalSetupUserRole.Validator };
            var validators = _journals.GetJournalSetupUsers(filter);
            if (validators.Count == 0)
                return new Guid[0];

            var validatorIds = validators
                .Where(x => x.UserIdentifier != excludeValidator)
                .Select(x => x.UserIdentifier)
                .ToArray();

            return validatorIds;
        }
    }
}