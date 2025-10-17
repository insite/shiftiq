using System;

using InSite.Domain.Records;

namespace InSite.Application.Records.Read
{
    public interface IRecordStore
    {
        void InsertRecord(GradebookCreated e);
        void DeleteRecord(GradebookDeleted e);
        void DeleteRecord(Guid aggregate);
        void UpdateRecord(GradebookCalculated e);
        void UpdateRecord(GradebookLocked e);
        void UpdateRecord(GradebookUnlocked e);
        void UpdateRecord(GradebookReferenced e);
        void UpdateRecord(GradebookRenamed e);
        void UpdateRecord(GradebookAchievementChanged e);
        void UpdateRecord(GradebookEventChanged e);
        void UpdateRecord(GradebookEventAdded e);
        void UpdateRecord(GradebookEventRemoved e);
        void UpdateRecord(GradebookTypeChanged e);
        void UpdateRecord(GradebookPeriodChanged e);
        void UpdateRecord(GradebookWarningAdded e);

        void InsertItem(GradeItemAdded e);
        void UpdateItem(GradeItemAchievementChanged e);
        void UpdateItem(GradeItemHookChanged e);
        void UpdateItem(GradeItemChanged e);
        void UpdateItem(GradeItemNotificationsChanged e);
        void UpdateItem(GradeItemPassPercentChanged e);
        void UpdateItem(GradeItemMaxPointsChanged e);
        void UpdateItem(GradeItemReferenced e);
        void UpdateItem(GradeItemCompetenciesChanged e);
        void DeleteItem(GradeItemDeleted e);
        void ReorderItems(GradeItemReordered e);

        void InsertEnrollment(EnrollmentStarted e);
        void DeleteEnrollment(GradebookUserDeleted e);
        void UpdateEnrollment(EnrollmentRestarted e);
        void UpdateEnrollment(EnrollmentCompleted e);
        void UpdateEnrollment(GradebookUserNoted e);
        void UpdateEnrollment(GradebookUserPeriodChanged e);

        void InsertValidation(GradebookValidationAdded e);
        void UpdateValidation(GradebookValidationChanged e);

        void InsertProgress(ProgressAdded e);
        void UpdateProgress(ProgressCommentChanged e);
        void UpdateProgress(ProgressCompleted2 e);
        void DeleteProgress(ProgressDeleted e);
        void UpdateProgress(ProgressHidden e);
        void UpdateProgress(ProgressIncompleted e);
        void UpdateProgress(ProgressLocked e);
        void UpdateProgress(ProgressNumberChanged e);
        void UpdateProgress(ProgressPercentChanged e);
        void UpdateProgress(ProgressPointsChanged e);
        void UpdateProgress(ProgressPublished e);
        void UpdateProgress(ProgressShowed e);
        void UpdateProgress(ProgressStarted e);
        void UpdateProgress(ProgressTextChanged e);
        void UpdateProgress(ProgressUnlocked e);
        void UpdateProgress(ProgressIgnored e);

        void DeleteOne(Guid gradebook);
        void DeleteAll();
    }
}
