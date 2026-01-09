using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Attempts
{
    [JsonConverter(typeof(AttemptQuestionConverter))]
    public abstract class AttemptQuestion
    {
        public Guid Identifier { get; set; }
        public int? Section { get; set; }
        public string Text { get; set; }
        public decimal? Points { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public QuestionItemType Type { get; set; }

        public decimal? CutScore { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public QuestionCalculationMethod CalculationMethod { get; set; }

        private class AttemptQuestionConverter : JsonConverter<AttemptQuestion>
        {
            public override bool CanWrite => false;

            public override AttemptQuestion ReadJson(JsonReader reader, Type type, AttemptQuestion value, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                    return null;

                if (reader.TokenType != JsonToken.StartObject)
                    throw new Exception("Wrong JsonToken: " + reader.TokenType.GetName());

                AttemptQuestion result;

                var jObj = JObject.Load(reader);
                var qType = jObj[nameof(Type)].Value<string>().ToEnum<QuestionItemType>();

                if (qType == QuestionItemType.Ordering)
                    result = new AttemptQuestionOrdering();
                else if (qType == QuestionItemType.Likert)
                    result = new AttemptQuestionLikert();
                else if (qType == QuestionItemType.Matching)
                    result = new AttemptQuestionMatch();
                else if (qType == QuestionItemType.ComposedVoice)
                    result = new AttemptQuestionComposedVoice();
                else if (qType.IsComposed())
                    result = new AttemptQuestionComposed();
                else if (qType.IsHotspot())
                    result = new AttemptQuestionHotspot();
                else
                    result = new AttemptQuestionDefault();

                serializer.Populate(jObj.CreateReader(), result);

                return result;
            }

            public override void WriteJson(JsonWriter writer, AttemptQuestion value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
