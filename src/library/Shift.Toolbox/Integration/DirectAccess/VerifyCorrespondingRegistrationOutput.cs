using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class VerifyCorrespondingRegistrationOutput
    {
        public bool Verified { get; set; }

        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }

        [JsonIgnore]
        public string Raw { get; set; }

        public static VerifyCorrespondingRegistrationOutput Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            if (IsValid(json))
            {
                var response = JsonConvert.DeserializeObject<VerifyCorrespondingRegistrationOutput>(json);
                response.Raw = json;
                return response;
            }

            var output = new VerifyCorrespondingRegistrationOutput
            {
                Raw = json,
                Reason = json,
                Verified = false
            };
            return output;
        }

        public static bool IsValid(string json)
        {
            try
            {
                var generator = new JSchemaGenerator();

                generator.GenerationProviders.Add(new StringEnumGenerationProvider());

                var schema = generator.Generate(typeof(VerifyCorrespondingRegistrationOutput));

                var parsed = JObject.Parse(json);

                if (parsed.IsValid(schema, out IList<string> errors))
                    return true;
            }
            catch (JsonException)
            {
                return false;
            }

            return false;
        }
    }
}