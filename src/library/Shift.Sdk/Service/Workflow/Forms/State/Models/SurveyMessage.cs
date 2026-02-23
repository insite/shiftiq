using System;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Surveys.Forms
{
    [Serializable]
    public class SurveyMessage
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public SurveyMessageType Type { get; set; }

        public Guid Identifier { get; set; }

        public SurveyMessage() { }
        public SurveyMessage(SurveyMessageType type, Guid identifier)
        {
            Type = type;
            Identifier = identifier;
        }
    }
}
