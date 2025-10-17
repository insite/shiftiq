using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Application.Contents.Read;

using Shift.Constant;

namespace InSite.Application.Records.Read
{
    public interface IJournalSearch
    {
        LogbookEnrollmentStatus GetEnrollmentStatus(Guid journalSetupIdentifier, Guid userIdentifier);
        int CountLearnerJournals(Guid organizationIdentifier, Guid userIdentifier);
        List<UserJournalDetail> GetEnrolledJournals(Guid organizationIdentifier, Guid userIdentifier, string language);
        List<UserJournalDetail> GetLearnerJournals(Guid organizationIdentifier, Guid userIdentifier, string language);
        QJournal GetJournal(Guid journalIdentifier, params Expression<Func<QJournal, object>>[] includes);
        QJournal GetJournal(Guid journalSetupIdentifier, Guid userIdentifier, params Expression<Func<QJournal, object>>[] includes);
        bool JournalExists(QJournalFilter filter);
        int CountJournals(QJournalFilter filter);
        List<QJournal> GetJournals(QJournalFilter filter, params Expression<Func<QJournal, object>>[] includes);

        List<EntrySummaryItem> GetEntrySummary(QExperienceFilter filter);
        QExperienceCompetency GetExperienceCompetency(Guid experienceIdentifier, Guid competencyIdentifier, params Expression<Func<QExperienceCompetency, object>>[] includes);
        int CountExperienceCompetencies(QExperienceCompetencyFilter filter);
        int CountExperienceCompetenciesFrameworks(QExperienceCompetencyFilter filter);
        List<QExperienceCompetency> GetExperienceCompetencies(QExperienceCompetencyFilter filter, params Expression<Func<QExperienceCompetency, object>>[] includes);
        List<QExperienceCompetency> GetExperienceCompetencies(Guid experienceIdentifier, params Expression<Func<QExperienceCompetency, object>>[] includes);
        List<QExperienceCompetency> GetExperienceCompetencies(Expression<Func<QExperienceCompetency, bool>> filter, params Expression<Func<QExperienceCompetency, object>>[] includes);
        QExperience GetExperience(Guid experienceIdentifier, params Expression<Func<QExperience, object>>[] includes);
        bool ExperienceExists(QExperienceFilter filter);
        int CountExperiences(QExperienceFilter filter);
        List<QExperience> GetExperiences(QExperienceFilter filter, params Expression<Func<QExperience, object>>[] includes);

        QComment GetJournalComment(Guid commentIdentifier);
        List<QComment> GetJournalComments(Guid journalIdentifier);

        int CountJournalSetups(QJournalSetupFilter filter);
        List<QJournalSetup> GetJournalSetups(IEnumerable<Guid> ids, params Expression<Func<QJournalSetup, object>>[] includes);
        List<QJournalSetup> GetJournalSetups(QJournalSetupFilter filter, params Expression<Func<QJournalSetup, object>>[] includes);
        QJournalSetup GetJournalSetup(Guid journalSetupIdentifier, params Expression<Func<QJournalSetup, object>>[] includes);

        int GetNextFieldSequence(Guid journalSetupIdentifier);
        QJournalSetupField GetJournalSetupField(Guid journalSetupFieldIdentifier, params Expression<Func<QJournalSetupField, object>>[] includes);
        QJournalSetupField GetJournalSetupField(Guid journalSetupIdentifier, string fieldType, params Expression<Func<QJournalSetupField, object>>[] includes);
        List<QJournalSetupField> GetJournalSetupFields(Guid journalSetupIdentifier);

        QCompetencyRequirement GetCompetencyRequirement(Guid journalSetupIdentifier, Guid standardIdentifier, params Expression<Func<QCompetencyRequirement, object>>[] includes);
        List<QCompetencyRequirement> GetCompetencyRequirements(Guid journalSetupIdentifier, params Expression<Func<QCompetencyRequirement, object>>[] includes);
        List<QCompetencyRequirement> GetCompetencyRequirements(Expression<Func<QCompetencyRequirement, bool>> filter, params Expression<Func<QCompetencyRequirement, object>>[] includes);
        int CountCompetencyRequirements(Expression<Func<QCompetencyRequirement, bool>> filter);

        bool ExistsJournalSetupUser(Guid journalSetupIdentifier, Guid userIdentifier, JournalSetupUserRole role);
        QJournalSetupUser GetJournalSetupUser(Guid journalSetupIdentifier, Guid userIdentifier, JournalSetupUserRole role, params Expression<Func<QJournalSetupUser, object>>[] includes);
        int CountJournalSetupUsers(VJournalSetupUserFilter filter);
        List<VJournalSetupUser> GetJournalSetupUsers(VJournalSetupUserFilter filter);
        List<JournalSetupUserExtended> GetJournalSetupUsersExtended(VJournalSetupUserFilter filter);

        bool ExistsJournalSetupGroup(Guid journalSetupId, Guid groupId);
        int CountJournalSetupGroups(QJournalSetupGroupFilter filter);
        List<JournalSetupGroupDetail> GetJournalSetupGroupDetails(QJournalSetupGroupFilter filter);

        QAreaRequirement GetAreaRequirement(Guid journalSetupId, Guid standardId, params Expression<Func<QAreaRequirement, object>>[] includes);
        List<QAreaRequirement> GetAreaRequirements(Guid journalSetupId, params Expression<Func<QAreaRequirement, object>>[] includes);
    }
}