using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class UserStatusStandard
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid DepartmentIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid StandardIdentifier { get; set; }
        public string StandardTitle { get; set; }
        public string StandardMnemonic { get; set; }
        public string StandardMetadata { get; set; }
        public string ValidationStatus { get; set; }
        public string StatisticName { get; set; }
        public int CountCP { get; set; }
        public int CountEX { get; set; }
        public int CountNC { get; set; }
        public int CountNA { get; set; }
        public int CountNT { get; set; }
        public int CountSA { get; set; }
        public int CountSV { get; set; }
        public int CountVA { get; set; }
        public int CountVN { get; set; }
        public int CountRQ { get; set; }
    }
}
