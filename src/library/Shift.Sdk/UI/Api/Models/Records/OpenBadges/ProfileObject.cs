using System;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn), JsonConverter(typeof(ProfileConverter))]
    public sealed class ProfileObject : BaseLinkedData
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value.NullIfWhiteSpace());
        }

        [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Uri Url
        {
            get => GetValue<Uri>();
            set => SetValue(value);
        }

        [JsonProperty("telephone", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Telephone
        {
            get => GetValue<string>();
            set => SetValue(value.NullIfWhiteSpace());
        }

        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description
        {
            get => GetValue<string>();
            set => SetValue(value.NullIfWhiteSpace());
        }

        [JsonProperty("image", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Uri Image
        {
            get => GetValue<Uri>();
            set => SetValue(value);
        }

        [JsonProperty("email", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Email
        {
            get => GetValue<string>();
            set => SetValue(value.NullIfWhiteSpace());
        }

        [JsonConstructor]
        private ProfileObject()
        {

        }

        private ProfileObject(Uri id)
            : base(id)
        {

        }

        public ProfileObject(string id, ProfileType type)
            : this(GetUrl(id))
        {
            Context = DefaultContext;
            Type = type.GetName();
        }

        protected override void LoadLinkedObjects()
        {

        }

        public override void Verify()
        {
            base.Verify();

            VerifyHostedData();
        }

        public override void VerifyObject()
        {
            if (Context != DefaultContext)
                throw ApplicationError.Create("The badge context is invalid");

            VerifyRequiredField(nameof(Type));

            if (Type == ProfileType.Issuer.GetName())
            {
                VerifyRequiredField(nameof(Name));
                VerifyRequiredField(nameof(Url));
                VerifyRequiredField(nameof(Email));
            }
        }

        private void VerifyHostedData()
        {
            var hostedJson = GetHostedData();
            var hostedObj = JsonConvert.DeserializeObject<ProfileObject>(hostedJson);

            hostedObj.VerifyObject();

            const string fieldPrefix = "Badge.";

            VerifyFieldValue(Name, hostedObj.Name, fieldPrefix + nameof(Name), false);
            VerifyFieldValue(Url, hostedObj.Url, fieldPrefix + nameof(Url), false);
            VerifyFieldValue(Telephone, hostedObj.Telephone, fieldPrefix + nameof(Telephone), false);
            VerifyFieldValue(Description, hostedObj.Description, fieldPrefix + nameof(Description), false);
            VerifyFieldValue(Image, hostedObj.Image, fieldPrefix + nameof(Image), false);
            VerifyFieldValue(Email, hostedObj.Email, fieldPrefix + nameof(Email), false);
        }

        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            if (Context.IsEmpty())
                throw CreateSerializationRequiredFieldError(nameof(Context));

            if (Type.IsEmpty())
                throw CreateSerializationRequiredFieldError(nameof(Type));

            if (Type == ProfileType.Issuer.GetName())
            {
                if (Name.IsEmpty())
                    throw CreateSerializationRequiredFieldError(nameof(Name));

                if (Url == null)
                    throw CreateSerializationRequiredFieldError(nameof(Url));

                if (Email.IsEmpty())
                    throw CreateSerializationRequiredFieldError(nameof(Email));
            }
        }

        private class ProfileConverter : BaseLinkedDataConverter<ProfileObject>
        {
            protected override ProfileObject CreateInstance(string id)
            {
                return new ProfileObject(GetUrl(id));
            }
        }
    }
}