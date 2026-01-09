using System;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class GradebookSettings
    {
        public int? DefaultPassPercent { get; set; }
        public bool HideIgnoreScoreCheckbox { get; set; }

        public bool IsEqual(GradebookSettings other)
        {
            return DefaultPassPercent == other.DefaultPassPercent
                && HideIgnoreScoreCheckbox == other.HideIgnoreScoreCheckbox;
        }
    }
}
