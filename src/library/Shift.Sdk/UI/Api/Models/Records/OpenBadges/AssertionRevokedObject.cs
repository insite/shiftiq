using System;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn), JsonConverter(typeof(AssertionRevokedConverter))]
    public sealed class AssertionRevokedObject : BaseLinkedData
    {
        [JsonProperty("revoked")]
        public bool Revoked
        {
            get => GetValue<bool>();
            private set => SetValue(value);
        }

        [JsonProperty("revocationReason", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RevocationReason
        {
            get => GetValue<string>();
            set => SetValue(value.NullIfWhiteSpace());
        }

        [JsonConstructor]
        private AssertionRevokedObject()
        {

        }

        private AssertionRevokedObject(Uri id)
            : base(id)
        {

        }

        public AssertionRevokedObject(string id)
            : this(GetUrl(id))
        {
            Context = "https://w3id.org/openbadges/v2";
            Type = "Assertion";
            Revoked = true;
        }

        protected override void LoadLinkedObjects()
        {

        }

        public override void VerifyObject()
        {

        }

        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            if (Revoked != true)
                throw CreateSerializationRequiredFieldError(nameof(Context));
        }

        private class AssertionRevokedConverter : BaseLinkedDataConverter<AssertionRevokedObject>
        {
            protected override AssertionRevokedObject CreateInstance(string id)
            {
                throw new NotImplementedException();
            }

            public override AssertionRevokedObject ReadJson(JsonReader reader, Type type, AssertionRevokedObject value, bool hasExistingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}