using System;

using Newtonsoft.Json;

namespace InSite.Persistence.Plugin.CMDS
{
    public class TUserStatus
    {
        public Guid JoinIdentifier { get; set; }

        public string DepartmentName { get; set; }
        public string DepartmentRole { get; set; }
        public string ItemName { get; set; }
        public string ListDomain { get; set; }
        public string ListFolder { get; set; }
        public string TagCriticality { get; set; }
        public string TagNecessity { get; set; }
        public string TagPrimacy { get; set; }
        public string OrganizationName { get; set; }
        public string UserName { get; set; }

        public int CountCP { get; set; }
        public int CountEX { get; set; }
        public int CountNA { get; set; }
        public int CountNC { get; set; }
        public int CountNT { get; set; }
        public int CountRQ { get; set; }
        public int CountSA { get; set; }
        public int CountSV { get; set; }
        public int CountVA { get; set; }
        public int CountVN { get; set; }
        public Guid DepartmentIdentifier { get; set; }
        public int ItemNumber { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public decimal? Progress { get; set; }
        public decimal? Score { get; set; }

        [JsonIgnore]
        public DateTime AsAt { get; set; }

        // API callers were given AsAt as a date/time offset before the column became a date, so they still
        // get one: midnight Mountain Time on the snapshot date. AsAtDate is the snapshot date on its own.
        [JsonProperty("AsAt")]
        public DateTimeOffset AsAtMountainTime
        {
            get
            {
                if (AsAt == default)
                    return default;

                var offset = MountainTime.GetUtcOffset(AsAt);

                return new DateTimeOffset(AsAt, offset);
            }
        }

        [JsonProperty("AsAtDate")]
        [JsonConverter(typeof(SnapshotDateConverter))]
        public DateTime AsAtDate => AsAt;

        private static readonly TimeZoneInfo MountainTime = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
    }
}
