using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Attempts
{
    public class AttemptConfiguration
    {
        public const int DefaultPingInterval = 30;

        public int PingInterval { get; set; } = DefaultPingInterval;
        public int? TimeLimit { get; set; }
        public string Language { get; set; }

        public bool SectionsAsTabs { get; set; }
        public bool TabNavigation { get; set; }
        public bool SingleQuestionPerTab { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public SpecificationTabTimeLimit TabTimeLimit { get; set; }

        public AttemptConfiguration Clone()
        {
            return (AttemptConfiguration)MemberwiseClone();
        }
    }
}
