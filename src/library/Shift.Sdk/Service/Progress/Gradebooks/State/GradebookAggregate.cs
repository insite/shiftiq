using System;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Shift.Constant;

namespace InSite.Domain.Records
{
    public class GradebookAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new GradebookState();
        public GradebookState Data => (GradebookState)State;

        public void AddItem(
            Guid item,
            string code,
            string name,
            string shortName,
            bool isReported,
            GradeItemFormat format,
            GradeItemType type,
            GradeItemWeighting weighting,
            decimal? passPercent,
            Guid? parent
            )
        {
            if (string.IsNullOrEmpty(shortName))
                shortName = null;

            var sequence = parent.HasValue
                ? Data.FindItem(parent.Value).Children.Count + 1
                : Data.RootItems.Count + 1;

            Apply(new GradeItemAdded(item, code, name, shortName, isReported, format, type, weighting, passPercent, parent, sequence));
        }

        public void AddEnrollment(Guid enrollment, Guid learner, Guid? period, DateTimeOffset? time, string comment)
        {
            Apply(new EnrollmentStarted(enrollment, learner, period, time, comment));
        }

        public void AddWarning(string warning)
        {
            Apply(new GradebookWarningAdded(warning));
        }

        public void AddValidation(Guid user, Guid competency, decimal? point)
        {
            Apply(new GradebookValidationAdded(user, competency, point));
        }

        public void CalculateRecord(Guid[] learners)
        {
            Apply(new GradebookCalculated(learners));
        }

        public void ChangeRecordAchievement(Guid? achievement)
        {
            Apply(new GradebookAchievementChanged(achievement));
        }

        public void AddGradebookEvent(Guid @event, bool isPrimary)
        {
            Apply(new GradebookEventAdded(@event, isPrimary));
        }

        public void RemoveGradebookEvent(Guid @event, Guid? newPrimaryEvent)
        {
            Apply(new GradebookEventRemoved(@event, newPrimaryEvent));
        }

        public void ChangeRecordType(GradebookType type, Guid? framework)
        {
            Apply(new GradebookTypeChanged(type, framework));
        }

        public void ChangeGradebookPeriod(Guid? period)
        {
            Apply(new GradebookPeriodChanged(period));
        }

        public void ChangeGradebookUserPeriod(Guid user, Guid? period)
        {
            Apply(new GradebookUserPeriodChanged(user, period));
        }

        public void ChangeItem(
            Guid item,
            string code,
            string name,
            string shortName,
            bool isReported,
            GradeItemFormat format,
            GradeItemType type,
            GradeItemWeighting weighting,
            Guid? parent
            )
        {
            if (string.IsNullOrEmpty(shortName))
                shortName = null;

            Apply(new GradeItemChanged(item, code, name, shortName, isReported, format, type, weighting, parent));
        }

        public void ChangeItemAchievement(Guid item, GradeItemAchievement achievement)
        {
            Apply(new GradeItemAchievementChanged(item, achievement));
        }

        public void ChangeItemCompetencies(Guid item, Guid[] standards)
        {
            var uniqueStandards = standards.Distinct().ToArray();

            Apply(new GradeItemCompetenciesChanged(item, uniqueStandards));
        }

        public void ChangeItemHook(Guid item, string hook)
        {
            Apply(new GradeItemHookChanged(item, hook));
        }

        public void ChangeItemMaxPoints(Guid item, decimal? maxPoint)
        {
            Apply(new GradeItemMaxPointsChanged(item, maxPoint));
        }

        public void ChangeGradeItemNotifications(Guid item, Notification[] notifications)
        {
            Apply(new GradeItemNotificationsChanged(item, notifications));
        }

        public void ChangeItemPassPercent(Guid item, decimal? passPercent)
        {
            Apply(new GradeItemPassPercentChanged(item, passPercent));
        }

        public void ChangeValidation(Guid student, Guid standard, decimal? point)
        {
            Apply(new GradebookValidationChanged(student, standard, point));
        }

        public void CreateRecord(Guid organization, string title, GradebookType type, Guid? @class, Guid? achievement, Guid? framework)
        {
            Apply(new GradebookCreated(organization, title, type, @class, achievement, framework));
        }

        public void Lock()
        {
            Apply(new GradebookLocked());
        }

        public void NoteUser(Guid user, string note, DateTimeOffset? added)
        {
            Apply(new GradebookUserNoted(user, note, added));
        }

        public void ReferenceRecord(string reference)
        {
            Apply(new GradebookReferenced(reference));
        }

        public void ReferenceItem(Guid item, string reference)
        {
            Apply(new GradeItemReferenced(item, reference));
        }

        public void RestartEnrollment(Guid learner, DateTimeOffset? time)
        {
            Apply(new EnrollmentRestarted(learner, time));
        }

        public void DeleteItem(Guid item)
        {
            Apply(new GradeItemDeleted(item));
        }

        public void DeleteUser(Guid user)
        {
            Apply(new GradebookUserDeleted(user));
        }

        public void RenameRecord(string name)
        {
            Apply(new GradebookRenamed(name));
        }

        public void ReorderItem(Guid? item, Guid[] children)
        {
            Apply(new GradeItemReordered(item, children));
        }

        public void Unlock()
        {
            Apply(new GradebookUnlocked());
        }

        public void Delete()
        {
            Apply(new GradebookDeleted());
        }

        public void AddParts(Guid item, CalculationPart[] parts)
        {
            Apply(new GradeItemCalculationChanged(item, parts));
        }
    }
}