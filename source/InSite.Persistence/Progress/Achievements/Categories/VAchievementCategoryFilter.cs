using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class VAchievementCategoryFilter : Filter
    {
        public string OrganizationCode { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public string CategoryName { get; set; }
        public string AchievementLabel { get; set; }
    }
}
