using System;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class CriteriaObject : BaseObject
    {
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Type
        {
            get => GetValue<string>();
            set => SetValue(value.NullIfWhiteSpace());
        }

        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Uri Id
        {
            get => GetValue<Uri>();
            set => SetValue(value);
        }

        [JsonProperty("narrative", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Narrative
        {
            get => GetValue<string>();
            set => SetValue(value.NullIfWhiteSpace());
        }

        public override void VerifyObject()
        {
            if (Type.IsNotEmpty() && Type != "Criteria")
                throw ApplicationError.Create("Unexpected criteria type: " + Type);

            if (Id == null && Narrative == null)
                throw ApplicationError.Create("The criteria is empty");
        }

        internal void VerifyFieldValues(CriteriaObject other)
        {
            const string fieldPrefix = "Criteria.";

            VerifyFieldValue(Type, other.Type, fieldPrefix + nameof(Type), false);
            VerifyFieldValue(Id, other.Id, fieldPrefix + nameof(Id), false);
            VerifyFieldValue(Narrative, other.Narrative, fieldPrefix + nameof(Narrative), false);
        }

        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            if (Type.IsNotEmpty() && Type != "Criteria")
                throw CreateSerializationRequiredFieldError(nameof(Type));

            if (Id == null && Narrative == null)
                throw CreateSerializationRequiredFieldError(nameof(Narrative));
        }
    }
}