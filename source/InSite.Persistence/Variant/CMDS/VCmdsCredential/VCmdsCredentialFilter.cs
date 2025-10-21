using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class VCmdsCredentialFilter : Filter
    {
        public DateTimeOffsetRange CompletionDate { get; set; } = new DateTimeOffsetRange();
        public DateTimeOffsetRange ExpirationDate { get; set; } = new DateTimeOffsetRange();

        public bool? IsCompetencyTraining { get; set; }
        public bool? IsReportingDisabled { get; set; }

        public Guid? AchievementIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string ProgressionStatus { get; set; }
        public string AchievementTitle { get; set; }
        public string AchievementType { get; set; }
        public string NotAchievementType { get; set; }

        public VCmdsCredentialFilter Copy()
        {
            VCmdsCredentialFilter filter = new VCmdsCredentialFilter
            {
                CompletionDate = CompletionDate,
                ExpirationDate = ExpirationDate,
                IsCompetencyTraining = IsCompetencyTraining,
                ProgressionStatus = ProgressionStatus,
                AchievementTitle = AchievementTitle,
                AchievementType = AchievementType,
                NotAchievementType = NotAchievementType,
                UserIdentifier = UserIdentifier
            };

            return filter;
        }
    }
}
