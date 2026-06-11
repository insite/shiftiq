using System;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class NCSHASettings
    {
        public bool ShowLastYearToEveryone { get; set; }

        public bool IsEqual(NCSHASettings other)
        {
            return ShowLastYearToEveryone == other.ShowLastYearToEveryone;
        }
    }
}
