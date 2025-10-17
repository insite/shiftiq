using System;
using System.Linq;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn), JsonConverter(typeof(BadgeConverter))]
    public sealed class BadgeObject : BaseLinkedData
    {
        #region Classes (JsonConverter)

        private class BadgeConverter : BaseLinkedDataConverter<BadgeObject>
        {
            protected override BadgeObject CreateInstance(string id)
            {
                return new BadgeObject(GetUrl(id));
            }
        }

        #endregion

        #region Constants

        private const string DefaultType = "BadgeClass";

        #endregion

        [JsonProperty("name")]
        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value.NullIfWhiteSpace());
        }

        [JsonProperty("description")]
        public string Description
        {
            get => GetValue<string>();
            set => SetValue(value.NullIfWhiteSpace());
        }

        [JsonProperty("image")]
        public Uri Image
        {
            get => GetValue<Uri>();
            set => SetValue(value);
        }

        [JsonProperty("criteria")]
        public CriteriaObject Criteria
        {
            get => GetValue<CriteriaObject>();
            set => SetValue(value);
        }

        [JsonProperty("issuer")]
        public ProfileObject Issuer
        {
            get => GetValue<ProfileObject>();
            set => SetValue(value);
        }

        [JsonProperty("tags", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string[] Tags
        {
            get => GetValue<string[]>();
            set => SetValue(value?.Select(x => x.NullIfWhiteSpace()).Where(x => x != null).ToArray().NullIfEmpty());
        }

        #region Construction

        [JsonConstructor]
        private BadgeObject()
        {

        }

        private BadgeObject(Uri id)
            : base(id)
        {

        }

        public BadgeObject(string id)
            : this(GetUrl(id))
        {
            Context = DefaultContext;
            Type = DefaultType;
        }

        #endregion

        #region Methods (load hosted)

        protected override void LoadLinkedObjects()
        {
            Issuer?.Load();
        }

        #endregion

        #region Methods (verify)

        public override void Verify()
        {
            base.Verify();

            VerifyHostedData();
        }

        public override void VerifyObject()
        {
            VerifyOwnProperties();
            VerifyLinkedObjects();
        }

        private void VerifyOwnProperties()
        {
            if (Context != DefaultContext)
                throw ApplicationError.Create("The badge context is invalid");

            if (Type != DefaultType)
                throw ApplicationError.Create("The badge type is invalid");

            VerifyRequiredField(nameof(Name));
            VerifyRequiredField(nameof(Description));
            VerifyRequiredField(nameof(Image));
            VerifyRequiredField(nameof(Criteria));
            VerifyRequiredField(nameof(Issuer));
        }

        private void VerifyLinkedObjects()
        {
            Criteria.VerifyObject();
            Issuer.Verify();
        }

        private void VerifyHostedData()
        {
            var hostedJson = GetHostedData();
            var hostedObj = JsonConvert.DeserializeObject<BadgeObject>(hostedJson);

            hostedObj.VerifyObject();

            const string fieldPrefix = "Badge.";

            VerifyFieldValue(Name, hostedObj.Name, fieldPrefix + nameof(Name), false);
            VerifyFieldValue(Description, hostedObj.Description, fieldPrefix + nameof(Description), false);
            VerifyFieldValue(Image, hostedObj.Image, fieldPrefix + nameof(Image), false);
            VerifyFieldEnumerable(Tags, hostedObj.Tags, fieldPrefix + nameof(Tags), false);

            Criteria.VerifyFieldValues(hostedObj.Criteria);
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

            if (Name.IsEmpty())
                throw CreateSerializationRequiredFieldError(nameof(Name));

            if (Description.IsEmpty())
                throw CreateSerializationRequiredFieldError(nameof(Description));

            if (Image == null)
                throw CreateSerializationRequiredFieldError(nameof(Image));

            if (Criteria == null)
                throw CreateSerializationRequiredFieldError(nameof(Criteria));

            if (Issuer == null)
                throw CreateSerializationRequiredFieldError(nameof(Issuer));
        }

        #endregion
    }
}