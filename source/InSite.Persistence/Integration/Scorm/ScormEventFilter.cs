using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class ScormEventFilter : Filter
    {
        public Guid[] ActivityIdentifiers { get; set; }
    }
}