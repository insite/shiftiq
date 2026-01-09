using System;

using Shift.Common.Timeline.Changes;

using Shift.Constant;

namespace InSite.Domain.Records
{
    public class JournalSetupAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new JournalSetupState();
        public JournalSetupState Data => (JournalSetupState)State;

        public void AddCompetencyRequirement(Guid competency, decimal? hours, int? journalItems, int? skillRating, bool includeHoursToArea)
        {
            Apply(new CompetencyRequirementAdded(competency, hours, journalItems, skillRating, includeHoursToArea));
        }

        public void ChangeCompetencyRequirement(Guid competency, decimal? hours, int? journalItems, int? skillRating, bool includeHoursToArea)
        {
            var c = Data.FindCompetencyRequirement(competency);
            if (c == null
                || (
                    c.Hours == hours
                    && c.JournalItems == journalItems
                    && c.SkillRating == skillRating
                    && c.IncludeHoursToArea == includeHoursToArea
                )
            )
            {
                return;
            }

            Apply(new CompetencyRequirementChanged(competency, hours, journalItems, skillRating, includeHoursToArea));
        }

        public void DeleteCompetencyRequirement(Guid competency)
        {
            Apply(new CompetencyRequirementDeleted(competency));
        }

        public void ChangeAchievement(Guid? achievement)
        {
            Apply(new JournalSetupAchievementChanged(achievement));
        }

        public void ChangeContent(Shift.Common.ContentContainer content)
        {
            Apply(new JournalSetupContentChanged(content));
        }

        public void ChangeEvent(Guid? @event)
        {
            Apply(new JournalSetupEventChanged(@event));
        }

        public void Create(Guid organization, string name)
        {
            Apply(new JournalSetupCreated(organization, name));
        }

        public void Delete()
        {
            Apply(new JournalSetupDeleted());
        }

        public void AddJournalSetupField(Guid field, JournalSetupFieldType type, int sequence, bool isRequired)
        {
            Apply(new JournalSetupFieldAdded(field, type, sequence, isRequired));
        }

        public void ChangeJournalSetupField(Guid field, bool isRequired)
        {
            Apply(new JournalSetupFieldChanged(field, isRequired));
        }

        public void ChangeLockUnlockJournalSetup(DateTimeOffset? journalSetupLocked)
        {
            Apply(new LockUnlockJournalSetupChanged(journalSetupLocked));
        }

        public void ChangeJournalSetupFieldContent(Guid field, Shift.Common.ContentContainer content)
        {
            Apply(new JournalSetupFieldContentChanged(field, content));
        }

        public void DeleteJournalSetupField(Guid field)
        {
            Apply(new JournalSetupFieldDeleted(field));
        }

        public void ChangeFramework(Guid? framework)
        {
            Apply(new JournalSetupFrameworkChanged(framework));
        }

        public void ChangeIsValidationRequired(bool isValidationRequired)
        {
            Apply(new JournalSetupIsValidationRequiredChanged(isValidationRequired));
        }

        public void AllowLogbookDownload()
        {
            Apply(new LogbookDownloadAllowed());
        }

        public void DisallowLogbookDownload()
        {
            Apply(new LogbookDownloadDisallowed());
        }

        public void ModifyJournalSetupAreaHours(Guid area, decimal? hours)
        {
            var r = Data.FindAreaRequirement(area);
            if (r != null && r.Hours == hours)
                return;

            Apply(new JournalSetupAreaHoursModified(area, hours));
        }

        public void ChangeMessages(Guid? validatorMessage, Guid? learnerMessage, Guid? learnerAddedMessage)
        {
            Apply(new JournalSetupMessagesChanged(validatorMessage, learnerMessage, learnerAddedMessage));
        }

        public void Rename(string name)
        {
            Apply(new JournalSetupRenamed(name));
        }

        public void AddUser(Guid user, JournalSetupUserRole role)
        {
            Apply(new JournalSetupUserAdded(user, role));
        }

        public void DeleteUser(Guid user, JournalSetupUserRole role)
        {
            Apply(new JournalSetupUserDeleted(user, role));
        }

        public void CreateGroup(Guid group)
        {
            Apply(new JournalSetupGroupCreated(group));
        }

        public void RemoveGroup(Guid group)
        {
            Apply(new JournalSetupGroupRemoved(group));
        }

        public void ReorderFields((Guid, int)[] fields)
        {
            Apply(new JournalSetupFieldsReordered(fields));
        }
    }
}
