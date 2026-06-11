using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class CmdsCollegeCertificationRequested : Change
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public string ProfileNumber { get; set; }
        public string ProfileTitle { get; set; }
        public string Institution { get; set; }

        public string CoreHoursRequired { get; set; }
        public string CoreHoursCompleted { get; set; }
        public string NonCoreHoursRequired { get; set; }
        public string NonCoreHoursCompleted { get; set; }
    }
}