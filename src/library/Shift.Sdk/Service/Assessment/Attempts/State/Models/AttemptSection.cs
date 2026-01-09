using System;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Attempts
{
    public class AttemptSection
    {
        public Guid Identifier { get; set; }
        public bool ShowWarningNextTab { get; set; }
        public bool IsBreakTimer { get; set; }
        public int TimeLimit { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public FormSectionTimeType TimerType { get; set; }
    }
}
