using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class VerificationObject : BaseObject
    {
        public VerificationType Type
        {
            get => GetValue<VerificationType>();
            private set => SetValue(value);
        }

        [JsonProperty("type", Required = Required.Always)]
        private string JsonType
        {
            get => Type.GetName();
            set => Type = value.ToEnum<VerificationType>();
        }

        [JsonConstructor]
        private VerificationObject()
        {

        }

        public VerificationObject(VerificationType type)
        {
            Type = type;
        }

        public override void VerifyObject()
        {
            VerifyRequiredField(nameof(Type));
        }

        internal void VerifyFieldValues(VerificationObject other)
        {
            const string fieldPrefix = "Verification.";

            VerifyFieldValue(Type, other.Type, fieldPrefix + nameof(Type), false);
        }
    }
}