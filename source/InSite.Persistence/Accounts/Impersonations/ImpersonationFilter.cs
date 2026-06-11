using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class ImpersonationFilter : Filter
    {
        public DateTime? SinceDate { get; set; }
        public DateTime? BeforeDate { get; set; }
    }
}