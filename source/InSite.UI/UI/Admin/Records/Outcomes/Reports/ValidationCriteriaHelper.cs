using System.Collections.Generic;

using InSite.Application.Records.Read;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Records.Outcomes.Reports
{
    public static class ValidationCriteriaHelper
    {
        public class CriteriaItem
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public static List<CriteriaItem> GetCriteriaItems(QGradebookCompetencyValidationFilter filter)
        {
            var timeZone = CurrentSessionState.Identity.User.TimeZone;
            var result = new List<CriteriaItem>();

            if (filter.GradebookTitle != "")
                result.Add(new CriteriaItem { Name = "Gradebook Title", Value = filter.GradebookTitle });

            if (filter.GradebookCreatedSince.HasValue)
                result.Add(new CriteriaItem { Name = "Gradebook Created Since", Value = filter.GradebookCreatedSince.Value.Format(timeZone) });

            if (filter.GradebookCreatedBefore.HasValue)
                result.Add(new CriteriaItem { Name = "Gradebook Created Before", Value = filter.GradebookCreatedBefore.Value.Format(timeZone) });

            if (filter.EventTitle != "")
                result.Add(new CriteriaItem { Name = "Class Title", Value = filter.EventTitle });

            if (filter.EventScheduledSince.HasValue)
                result.Add(new CriteriaItem { Name = "Scheduled Since", Value = filter.EventScheduledSince.Value.Format(timeZone) });

            if (filter.EventScheduledBefore.HasValue)
                result.Add(new CriteriaItem { Name = "Scheduled Before", Value = filter.EventScheduledBefore.Value.Format(timeZone) });

            if (filter.AchievementTitle != "")
                result.Add(new CriteriaItem { Name = "Achievement Title", Value = filter.AchievementTitle });

            if (filter.StudentName != "")
                result.Add(new CriteriaItem { Name = "Student", Value = filter.StudentName });

            if (filter.EventInstructorIdentifier.HasValue)
                result.Add(new CriteriaItem { Name = "Class Instructor", Value = UserSearch.GetFullName(filter.EventInstructorIdentifier.Value) });

            if (filter.StudentEmployerGroupIdentifier.HasValue)
                result.Add(new CriteriaItem { Name = "Student Employer", Value = ServiceLocator.GroupSearch.GetGroup(filter.StudentEmployerGroupIdentifier.Value).GroupName });

            if (filter.CompetencyIdentifier.HasValue)
                result.Add(new CriteriaItem { Name = "Competency", Value = StandardSearch.Select(filter.CompetencyIdentifier.Value)?.ContentTitle });

            return result;
        }
    }
}