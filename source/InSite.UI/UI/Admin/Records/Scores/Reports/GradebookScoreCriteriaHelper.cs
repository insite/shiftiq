using System.Collections.Generic;

using InSite.Application.Records.Read;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Records.Scores.Reports
{
    public static class GradebookScoreCriteriaHelper
    {
        public class CriteriaItem
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public static List<CriteriaItem> GetCriteriaItems(QProgressFilter filter)
        {
            var timeZone = CurrentSessionState.Identity.User.TimeZone;
            var result = new List<CriteriaItem>();

            if (filter.GradebookTitle.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Gradebook Title", Value = filter.GradebookTitle });

            if (filter.GradebookCreatedSince.HasValue)
                result.Add(new CriteriaItem { Name = "Gradebook Created Since", Value = filter.GradebookCreatedSince.Value.Format(timeZone) });

            if (filter.GradebookCreatedBefore.HasValue)
                result.Add(new CriteriaItem { Name = "Gradebook Created Before", Value = filter.GradebookCreatedBefore.Value.Format(timeZone) });

            if (filter.ItemTypes.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Item Types", Value = string.Join(", ", filter.ItemTypes) });

            if (filter.ItemFormat.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Item Format", Value = filter.ItemFormat });

            if (filter.ItemName.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Item Name", Value = filter.ItemName });

            if (filter.ScorePercentFrom.HasValue)
                result.Add(new CriteriaItem { Name = "Score Percent From", Value = $"{filter.ScorePercentFrom:p3}" });

            if (filter.ScorePercentThru.HasValue)
                result.Add(new CriteriaItem { Name = "Score Percent Thru", Value = $"{filter.ScorePercentThru:p3}" });

            if (filter.ScoreText.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Score Text", Value = filter.ScoreText });

            if (filter.ScoreComment.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Score Comment", Value = filter.ScoreComment });

            if (filter.EventTitle.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Class Title", Value = filter.EventTitle });

            if (filter.EventScheduledSince.HasValue)
                result.Add(new CriteriaItem { Name = "Scheduled Since", Value = filter.EventScheduledSince.Value.Format(timeZone) });

            if (filter.EventScheduledBefore.HasValue)
                result.Add(new CriteriaItem { Name = "Scheduled Before", Value = filter.EventScheduledBefore.Value.Format(timeZone) });

            if (filter.AchievementTitle.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Achievement Title", Value = filter.AchievementTitle });

            if (filter.StudentName.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Student", Value = filter.StudentName });

            if (filter.EventInstructorIdentifier.HasValue)
                result.Add(new CriteriaItem { Name = "Class Instructor", Value = UserSearch.GetFullName(filter.EventInstructorIdentifier.Value) });

            if (filter.StudentEmployerGroupIdentifier.HasValue)
                result.Add(new CriteriaItem { Name = "Student Employer", Value = ServiceLocator.GroupSearch.GetGroup(filter.StudentEmployerGroupIdentifier.Value).GroupName });

            var studentEmployerGroupStatus = TCollectionItemCache.GetName(filter.StudentEmployerGroupStatusIdentifier);
            if (studentEmployerGroupStatus.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Employer Status", Value = studentEmployerGroupStatus });

            return result;
        }
    }
}