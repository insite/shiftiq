using Shift.Common.Timeline.Changes;

using InSite.Application.Contents.Read;
using InSite.Domain.Records;

using Shift.Constant;

namespace InSite.Application.Records.Read
{
    /// <summary>
    /// Implements the projector for Journal changes.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Changes can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from modifications to the projection tables). A processor,
    /// in contrast, should *never* replay past changes.
    /// </remarks>
    public class JournalChangeProjector
    {
        private readonly IJournalStore _store;
        private readonly IContentStore _contentStore;

        public JournalChangeProjector(IChangeQueue publisher, IJournalStore store, IContentStore contentStore)
        {
            _store = store;
            _contentStore = contentStore;

            publisher.Subscribe<CompetencyRequirementAdded>(Handle);
            publisher.Subscribe<CompetencyRequirementChanged>(Handle);
            publisher.Subscribe<CompetencyRequirementDeleted>(Handle);
            publisher.Subscribe<JournalSetupAchievementChanged>(Handle);
            publisher.Subscribe<JournalSetupAreaHoursModified>(Handle);
            publisher.Subscribe<JournalSetupContentChanged>(Handle);
            publisher.Subscribe<JournalSetupCreated>(Handle);
            publisher.Subscribe<JournalSetupDeleted>(Handle);
            publisher.Subscribe<JournalSetupEventChanged>(Handle);
            publisher.Subscribe<JournalSetupFieldAdded>(Handle);
            publisher.Subscribe<JournalSetupFieldChanged>(Handle);
            publisher.Subscribe<JournalSetupFieldContentChanged>(Handle);
            publisher.Subscribe<JournalSetupFieldDeleted>(Handle);
            publisher.Subscribe<JournalSetupFieldsReordered>(Handle);
            publisher.Subscribe<JournalSetupFrameworkChanged>(Handle);
            publisher.Subscribe<JournalSetupIsValidationRequiredChanged>(Handle);
            publisher.Subscribe<LogbookDownloadAllowed>(Handle);
            publisher.Subscribe<LogbookDownloadDisallowed>(Handle);
            publisher.Subscribe<LockUnlockJournalSetupChanged>(Handle);
            publisher.Subscribe<JournalSetupMessagesChanged>(Handle);
            publisher.Subscribe<JournalSetupRenamed>(Handle);
            publisher.Subscribe<JournalSetupUserAdded>(Handle);
            publisher.Subscribe<JournalSetupUserDeleted>(Handle);
            publisher.Subscribe<JournalSetupGroupCreated>(Handle);
            publisher.Subscribe<JournalSetupGroupRemoved>(Handle);

            publisher.Subscribe<CommentAdded>(Handle);
            publisher.Subscribe<CommentChanged>(Handle);
            publisher.Subscribe<CommentDeleted>(Handle);
            publisher.Subscribe<ExperienceAdded>(Handle);
            publisher.Subscribe<ExperienceCompetencyAdded>(Handle);
            publisher.Subscribe<ExperienceCompetencyChanged>(Handle);
            publisher.Subscribe<ExperienceCompetencyDeleted>(Handle);
            publisher.Subscribe<ExperienceCompetencySatisfactionLevelChanged>(Handle);
            publisher.Subscribe<ExperienceCompetencySkillRatingChanged>(Handle);
            publisher.Subscribe<ExperienceDeleted>(Handle);
            publisher.Subscribe<ExperienceCompletedChanged>(Handle);
            publisher.Subscribe<ExperienceEmployerChanged>(Handle);
            publisher.Subscribe<ExperienceEvidenceChanged>(Handle);
            publisher.Subscribe<ExperienceHoursChanged>(Handle);
            publisher.Subscribe<ExperienceInstructorChanged>(Handle);
            publisher.Subscribe<ExperienceSupervisorChanged>(Handle);
            publisher.Subscribe<ExperienceTimeChanged>(Handle);
            publisher.Subscribe<ExperienceTrainingChanged>(Handle);
            publisher.Subscribe<ExperienceValidated>(Handle);
            publisher.Subscribe<ExperienceMediaEvidenceChanged>(Handle);
            publisher.Subscribe<ExperienceCapturedEvidenceChanged>(Handle);
            publisher.Subscribe<JournalCreated>(Handle);
            publisher.Subscribe<JournalDeleted>(Handle);
        }

        public void Handle(CompetencyRequirementAdded e)
            => _store.InsertCompetencyRequirement(e);

        public void Handle(CompetencyRequirementChanged e)
            => _store.UpdateCompetencyRequirement(e);

        public void Handle(CompetencyRequirementDeleted e)
            => _store.DeleteCompetencyRequirement(e);

        public void Handle(JournalSetupAchievementChanged e)
            => _store.UpdateJournalSetup(e, x => x.AchievementIdentifier = e.Achievement);

        public void Handle(JournalSetupAreaHoursModified e)
            => _store.UpdateAreaRequirement(e);

        public void Handle(JournalSetupContentChanged e)
        {
            _contentStore.SaveContainer(e.OriginOrganization, ContentContainerType.JournalSetup, e.AggregateIdentifier, e.Content);
            _store.UpdateJournalSetup(e, null);
        }

        public void Handle(JournalSetupCreated e)
            => _store.InsertJournalSetup(e);

        public void Handle(JournalSetupDeleted e)
            => _store.DeleteJournalSetup(e);

        public void Handle(JournalSetupEventChanged e)
            => _store.UpdateJournalSetup(e, x => x.EventIdentifier = e.Event);

        public void Handle(JournalSetupFieldAdded e)
            => _store.InsertField(e);

        public void Handle(JournalSetupFieldChanged e)
        {
            _store.UpdateField(e, e.Field, x =>
            {
                x.FieldIsRequired = e.IsRequired;
            });
        }

        public void Handle(JournalSetupFieldContentChanged e)
        {
            _contentStore.SaveContainer(e.OriginOrganization, ContentContainerType.JournalSetupField, e.Field, e.Content);

            _store.UpdateField(e, e.Field, null);
        }

        public void Handle(JournalSetupFieldDeleted e)
            => _store.DeleteField(e);

        public void Handle(JournalSetupFieldsReordered e)
            => _store.ReorderFields(e);

        public void Handle(JournalSetupFrameworkChanged e)
            => _store.UpdateJournalSetup(e, x => x.FrameworkStandardIdentifier = e.Framework);

        public void Handle(LockUnlockJournalSetupChanged e)
            => _store.UpdateJournalSetup(e, x => x.JournalSetupLocked = e.JournalSetupLocked);

        public void Handle(JournalSetupIsValidationRequiredChanged e)
            => _store.UpdateJournalSetup(e, x => x.IsValidationRequired = e.IsValidationRequired);

        public void Handle(LogbookDownloadAllowed e)
            => _store.UpdateJournalSetup(e, x => x.AllowLogbookDownload = true);

        public void Handle(LogbookDownloadDisallowed e)
            => _store.UpdateJournalSetup(e, x => x.AllowLogbookDownload = false);

        public void Handle(JournalSetupMessagesChanged e)
            => _store.UpdateJournalSetup(e, x =>
                {
                    x.ValidatorMessageIdentifier = e.ValidatorMessage;
                    x.LearnerMessageIdentifier = e.LearnerMessage;
                    x.LearnerAddedMessageIdentifier = e.LearnerAddedMessage;
                }
            );

        public void Handle(JournalSetupRenamed e)
            => _store.UpdateJournalSetup(e, x => x.JournalSetupName = e.Name);

        public void Handle(JournalSetupUserAdded e)
            => _store.InsertUser(e);

        public void Handle(JournalSetupUserDeleted e)
            => _store.DeleteUser(e);

        public void Handle(JournalSetupGroupCreated e)
            => _store.InsertGroup(e);

        public void Handle(JournalSetupGroupRemoved e)
            => _store.DeleteGroup(e);

        public void Handle(CommentAdded e)
            => _store.InsertComment(e);

        public void Handle(CommentChanged e)
            => _store.UpdateComment(e);

        public void Handle(CommentDeleted e)
            => _store.DeleteComment(e);

        public void Handle(ExperienceAdded e)
            => _store.InsertExperience(e);

        public void Handle(ExperienceCompetencyAdded e)
            => _store.InsertExperienceCompetency(e);

        public void Handle(ExperienceCompetencyChanged e)
            => _store.UpdateExperienceCompetency(e.Experience, e.Competency, x => x.CompetencyHours = e.Hours);

        public void Handle(ExperienceCompetencyDeleted e)
            => _store.DeleteExperienceCompetency(e);

        public void Handle(ExperienceCompetencySatisfactionLevelChanged e)
            => _store.UpdateExperienceCompetency(
                e.Experience,
                e.Competency,
                x => x.SatisfactionLevel = e.SatisfactionLevel != ExperienceCompetencySatisfactionLevel.None
                    ? e.SatisfactionLevel.ToString()
                    : null
                );

        public void Handle(ExperienceCompetencySkillRatingChanged e)
            => _store.UpdateExperienceCompetency(e.Experience, e.Competency, x => x.SkillRating = e.SkillRating);

        public void Handle(ExperienceDeleted e)
            => _store.DeleteExperience(e);

        public void Handle(ExperienceCompletedChanged e)
            => _store.UpdateExperience(e.Experience, x => x.ExperienceCompleted = e.Completed);

        public void Handle(ExperienceEmployerChanged e)
            => _store.UpdateExperience(e.Experience, x => x.Employer = e.Employer);

        public void Handle(ExperienceEvidenceChanged e)
            => _store.UpdateExperience(e.Experience, x => x.ExperienceEvidence = e.Evidence);

        public void Handle(ExperienceHoursChanged e)
            => _store.UpdateExperience(e.Experience, x => x.ExperienceHours = e.Hours);

        public void Handle(ExperienceInstructorChanged e)
            => _store.UpdateExperience(e.Experience, x => x.Instructor = e.Instructor);

        public void Handle(ExperienceSupervisorChanged e)
            => _store.UpdateExperience(e.Experience, x => x.Supervisor = e.Supervisor);

        public void Handle(ExperienceTimeChanged e)
        {
            _store.UpdateExperience(e.Experience, x =>
            {
                x.ExperienceStarted = e.Started;
                x.ExperienceStopped = e.Stopped;
            });
        }

        public void Handle(ExperienceTrainingChanged e)
        {
            _store.UpdateExperience(e.Experience, x =>
            {
                x.TrainingLevel = e.Level;
                x.TrainingLocation = e.Location;
                x.TrainingProvider = e.Provider;
                x.TrainingCourseTitle = e.CourseTitle;
                x.TrainingComment = e.Comment;
                x.TrainingType = e.Type;
            });
        }

        public void Handle(ExperienceValidated e)
        {
            _store.UpdateExperience(e.Experience, x =>
            {
                x.ValidatorUserIdentifier = e.Validator;
                x.ExperienceValidated = e.Validated;
                x.SkillRating = e.SkillRating;
            });
        }

        public void Handle(ExperienceMediaEvidenceChanged e)
        {
            _store.UpdateExperience(e.Experience, x =>
            {
                x.MediaEvidenceName = e.Name;
                x.MediaEvidenceType = e.Type;
                x.MediaEvidenceFileIdentifier = e.FileIdentifier;
            });
        }

        public void Handle(ExperienceCapturedEvidenceChanged e)
        {
            // Obsolete
        }

        public void Handle(JournalCreated e)
            => _store.InsertJournal(e);

        public void Handle(JournalDeleted e)
            => _store.DeleteJournal(e);
    }
}
