using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class CmdsAchievementChanged : Change
    {
        public Guid OrganizationIdentifier { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public int Lifetime { get; set; }
        public string Description { get; set; }
        public decimal Hours { get; set; }

        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public string Change { get; set; }
    }
}