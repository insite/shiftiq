using System;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class StandardSettings
    {
        public bool ShowStandardCategories { get; set; }

        public bool IsEqual(StandardSettings other)
        {
            return ShowStandardCategories == other.ShowStandardCategories;
        }
    }
}
