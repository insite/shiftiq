using System;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn), JsonConverter(typeof(AssertionConverter))]
    public sealed class AssertionObject : BaseLinkedData
    {
        #region Classes (JsonConverter)

        private class AssertionConverter : BaseLinkedDataConverter<AssertionObject>
        {
            protected override AssertionObject CreateInstance(string id)
            {
                return new AssertionObject(GetUrl(id));
            }
        }

        #endregion

        #region Constants

        private const string DefaultType = "Assertion";

        #endregion

        [JsonProperty("issuedOn")]
        public DateTimeOffset IssuedOn
        {
            get => GetValue<DateTimeOffset>();
            set => SetValue(value);
        }

        [JsonProperty("expires", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTimeOffset? Expires
        {
            get => GetValue<DateTimeOffset?>();
            set => SetValue(value);
        }

        [JsonProperty("revoked", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Revoked
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        [JsonProperty("revocationReason", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RevocationReason
        {
            get => GetValue<string>();
            set => SetValue(value.NullIfWhiteSpace());
        }

        [JsonProperty("recipient")]
        public IdentityObject Recipient
        {
            get => GetValue<IdentityObject>();
            set => SetValue(value);
        }

        [JsonProperty("badge")]
        public BadgeObject Badge
        {
            get => GetValue<BadgeObject>();
            set => SetValue(value);
        }

        [JsonProperty("verification")]
        public VerificationObject Verification
        {
            get => GetValue<VerificationObject>();
            set => SetValue(value);
        }

        #region Construction

        [JsonConstructor]
        private AssertionObject()
        {

        }

        private AssertionObject(Uri id)
            : base(id)
        {

        }

        public AssertionObject(string id)
            : this(GetUrl(id))
        {
            Context = DefaultContext;
            Type = DefaultType;
        }

        #endregion

        #region Methods (load hosted)

        protected override void LoadLinkedObjects()
        {
            Badge?.Load();
        }

        #endregion

        #region Methods (verify)

        public override void Verify()
        {
            base.Verify();

            if (Verification.Type == VerificationType.HostedBadge)
                VerifyHostedData();
            else
                throw ApplicationError.Create("Hosted verification is the only type of verification supported");
        }

        public override void VerifyObject()
        {
            VerifyOwnProperties();
            VerifyLinkedObjects();
        }

        private void VerifyOwnProperties()
        {
            if (Revoked)
            {
                var message = "The assertion is revoked";
                if (RevocationReason.IsNotEmpty())
                    message += ": " + RevocationReason;
                throw ApplicationError.Create(message);
            }

            if (Context != DefaultContext)
                throw ApplicationError.Create("The assertion context is invalid");

            if (Type != DefaultType)
                throw ApplicationError.Create("The assertion type is invalid");

            VerifyRequiredField(nameof(IssuedOn));
            VerifyRequiredField(nameof(Recipient));
            VerifyRequiredField(nameof(Badge));
            VerifyRequiredField(nameof(Verification));

            if (IssuedOn > DateTimeOffset.UtcNow)
                throw ApplicationError.Create("The date of issue of the assertion is invalid");

            if (Expires.HasValue)
            {
                if (Expires.Value <= IssuedOn)
                    throw ApplicationError.Create("The expiration date of the assertion is not valid");

                if (Expires.Value <= DateTimeOffset.UtcNow)
                    throw ApplicationError.Create("The assertion is expired");
            }
        }

        private void VerifyLinkedObjects()
        {
            Verification.VerifyObject();
            Recipient.VerifyObject();
            Badge.Verify();
        }

        private void VerifyHostedData()
        {
            var hostedJson = GetHostedData();
            var hostedObj = JsonConvert.DeserializeObject<AssertionObject>(hostedJson);

            hostedObj.VerifyObject();

            const string fieldPrefix = "Assertion.";

            VerifyFieldValue(IssuedOn, hostedObj.IssuedOn, fieldPrefix + nameof(IssuedOn), false);
            VerifyFieldValue(Expires, hostedObj.Expires, fieldPrefix + nameof(Expires), false);

            Verification.VerifyFieldValues(hostedObj.Verification);
            Recipient.VerifyFieldValues(hostedObj.Recipient);
        }

        #endregion

        #region Methods ((de)serialize)

        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            if (Context.IsEmpty())
                throw CreateSerializationRequiredFieldError(nameof(Context));

            if (Type.IsEmpty())
                throw CreateSerializationRequiredFieldError(nameof(Type));

            if (!HasValue(nameof(IssuedOn)) || IssuedOn == default)
                throw CreateSerializationRequiredFieldError(nameof(IssuedOn));

            if (Recipient == null)
                throw CreateSerializationRequiredFieldError(nameof(Recipient));

            if (Badge == null)
                throw CreateSerializationRequiredFieldError(nameof(Badge));

            if (Verification == null)
                throw CreateSerializationRequiredFieldError(nameof(Verification));

            if (Verification.Type != VerificationType.HostedBadge)
                throw ApplicationError.Create(Verification.Type.GetName() + " is not supported");
        }

        #endregion
    }
}