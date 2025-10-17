using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Events
{
    public class EventInstruction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public EventInstructionType Type { get; set; }

        public MultilingualString Text { get; set; }
    }
}
