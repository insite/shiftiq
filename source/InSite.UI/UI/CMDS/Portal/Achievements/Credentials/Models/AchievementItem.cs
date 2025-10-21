using System;

using Shift.Common;

namespace InSite.UI.CMDS.Portal.Achievements.Credentials
{
    public class AchievementItem
    {
        public Guid? ExperienceIdentifier { get; set; }
        public Guid? CredentialIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid? AchievementIdentifier { get; set; }
        public string AuthorityLocation { get; set; }
        public string AccreditorName { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset? DateCompleted { get; set; }
        public DateTimeOffset? DateExpired { get; set; }
        public decimal? Hours { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsTimeSensitive { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public string Score { get; set; }
        public string ValidationStatus { get; set; }
        public int? LifetimeMonths { get; set; }
        public bool? IsInTrainingPlan { get; set; }
        public string ProgramTitle { get; set; }
        public string AchievementTitle { get; set; }
        public string AchievementDescription { get; set; }
        public string AchievementType { get; set; }
        public string EmployeeLastFirstName { get; set; }
        public string EmployeeFirstLastName { get; set; }
        public bool? IsSuccess { get; set; }
        public bool? IsSkillsPassport { get; set; }

        public string DateCompletedHtml
        {
            get
            {
                if (!DateCompleted.HasValue)
                    return string.Empty;

                var tz = CurrentSessionState.Identity.User.TimeZone;
                var date = TimeZones.FormatDateOnly(DateCompleted.Value, tz);
                var time = TimeZones.FormatTimeOnly(DateCompleted.Value, tz);
                return $"<span title='{time}'>{date}</span>";
            }
        }

        public string DateExpiredHtml
        {
            get
            {
                if (!DateExpired.HasValue)
                    return string.Empty;

                var tz = CurrentSessionState.Identity.User.TimeZone;
                var date = TimeZones.FormatDateOnly(DateExpired.Value, tz);
                var time = TimeZones.FormatTimeOnly(DateExpired.Value, tz);
                return $"<span title='{time}'>{date}</span>";
            }
        }
    }
}