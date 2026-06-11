using System;

namespace InSite.Persistence
{
    [Serializable]
    public class StandardValidationChangeFilter : StandardValidationFilter
    {
        public DateTimeOffset? ChangePostedSince { get; set; }
        public DateTimeOffset? ChangePostedBefore { get; set; }
    }
}
