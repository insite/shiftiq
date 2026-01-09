using System;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Records;

namespace InSite.Application.Records.Read
{
    public interface IJournalStore
    {
        void InsertJournalSetup(JournalSetupCreated e);
        void UpdateJournalSetup(IChange e, Action<QJournalSetup> change);
        void DeleteJournalSetup(JournalSetupDeleted e);

        void InsertCompetencyRequirement(CompetencyRequirementAdded e);
        void UpdateCompetencyRequirement(CompetencyRequirementChanged e);
        void DeleteCompetencyRequirement(CompetencyRequirementDeleted e);

        void UpdateAreaRequirement(JournalSetupAreaHoursModified e);

        void InsertField(JournalSetupFieldAdded e);
        void UpdateField(IChange e, Guid journalSetupFieldIdentifier, Action<QJournalSetupField> action);
        void DeleteField(JournalSetupFieldDeleted e);
        void ReorderFields(JournalSetupFieldsReordered e);

        void InsertUser(JournalSetupUserAdded e);
        void DeleteUser(JournalSetupUserDeleted e);
        void InsertGroup(JournalSetupGroupCreated e);
        void DeleteGroup(JournalSetupGroupRemoved e);

        void InsertJournal(JournalCreated e);
        void DeleteJournal(JournalDeleted e);

        void InsertComment(CommentAdded e);
        void UpdateComment(CommentChanged e);
        void DeleteComment(CommentDeleted e);

        void InsertExperience(ExperienceAdded e);
        void UpdateExperience(Guid experienceIdentifier, Action<QExperience> action);
        void DeleteExperience(ExperienceDeleted e);

        void InsertExperienceCompetency(ExperienceCompetencyAdded e);
        void UpdateExperienceCompetency(Guid experienceIdentifier, Guid competencyIdentifier, Action<QExperienceCompetency> action);
        void DeleteExperienceCompetency(ExperienceCompetencyDeleted e);
    }
}
