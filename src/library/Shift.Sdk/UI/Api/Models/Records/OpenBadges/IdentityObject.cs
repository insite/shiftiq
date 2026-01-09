using System;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class IdentityObject : BaseObject
    {
        public IdentityType Type
        {
            get => GetValue<IdentityType>();
            private set => SetValue(value);
        }

        public string Identity
        {
            get => GetValue<string>();
            private set => SetValue(value.NullIfWhiteSpace());
        }

        [JsonProperty("salt", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Salt
        {
            get => GetValue<string>();
            private set => SetValue(value.NullIfWhiteSpace());
        }

        [JsonProperty(PropertyName = "hashed")]
        public bool IsHashed
        {
            get => GetValue<bool>();
            set
            {
                if (!SetValue(value))
                    return;

                if (value == true && Salt == null)
                    Salt = RandomStringGenerator.Create(RandomStringType.AlphanumericCaseSensitive, 32);
            }
        }

        private bool IsSha256Hash => Identity.StartsWith("sha256$", StringComparison.OrdinalIgnoreCase);

        private bool IsMd5Hash => Identity.StartsWith("md5$", StringComparison.OrdinalIgnoreCase);

        #region Properties (JSON only)

        [JsonProperty("type")]
        private string JsonType
        {
            get => Type.GetName().ToLower();
            set => Type = value.ToEnum<IdentityType>();
        }

        [JsonProperty("identity")]
        private string JsonIdentity
        {
            get => IsParsed || !IsHashed ? Identity : "sha256$" + StringHelper.CreateHashSha256(Identity + Salt).ToLower();
            set => Identity = value;
        }

        #endregion

        #region Construction

        [JsonConstructor]
        private IdentityObject()
        {

        }

        public IdentityObject(IdentityType type, string identity, bool isHashed)
        {
            Type = type;
            Identity = identity.NullIfWhiteSpace() ?? throw new ArgumentNullException(nameof(identity));
            IsHashed = isHashed;
        }

        #endregion

        #region Methods (verify)

        public bool Verify(string identity)
        {
            if (identity.IsEmpty())
                return false;

            if (!IsParsed || !IsHashed)
                return StringHelper.Equals(Identity, identity);

            Func<string, byte[], byte[], bool> verify;
            byte[] hash;

            if (IsSha256Hash)
            {
                verify = StringHelper.VerifyHashSha256;
                hash = StringHelper.HexToByteArray(Identity.Substring(7));
            }
            else if (IsMd5Hash)
            {
                verify = StringHelper.VerifyHashMd5;
                hash = StringHelper.HexToByteArray(Identity.Substring(4));
            }
            else
                throw ApplicationError.Create("Unsupported hashing algorithm: " + Identity);

            return verify(identity + Salt, hash, null);
        }

        public override void VerifyObject()
        {
            VerifyRequiredField(nameof(Type));
            VerifyRequiredField(nameof(Identity));
            VerifyRequiredField(nameof(IsHashed));

            if (IsHashed && !IsSha256Hash && !IsMd5Hash)
                throw ApplicationError.Create("The identitiy uses an unsupported hash type");
        }

        public void VerifyFieldValues(IdentityObject other)
        {
            const string fieldPrefix = "Recipient.";

            VerifyFieldValue(Type, other.Type, fieldPrefix + nameof(Type), false);
            VerifyFieldValue(Identity, other.Identity, fieldPrefix + nameof(Identity), false);
            VerifyFieldValue(IsHashed, other.IsHashed, fieldPrefix + nameof(IsHashed), false);

            if (IsHashed)
                VerifyFieldValue(Salt, other.Salt, fieldPrefix + nameof(Salt), false);
        }

        #endregion

        #region Methods ((de)serialize)

        public bool ShouldSerializeSalt() => IsHashed;

        #endregion
    }
}