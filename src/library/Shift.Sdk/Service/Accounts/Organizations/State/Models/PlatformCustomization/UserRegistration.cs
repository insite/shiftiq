using System;
using System.ComponentModel;
using System.Runtime.Serialization;

using InSite.Domain.Accounts.Tenants.Models;

using Newtonsoft.Json;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class UserRegistration
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public UserRegistrationMode RegistrationMode { get; set; }

        public bool AutomaticApproval { get; set; }

        [DefaultValue(true)]
        public bool ConvertProvinceAbbreviation { get; set; } = true;

        public UserRegistrationFieldMask FieldMask { get; set; }

        public UserRegistration()
        {
            FieldMask = new UserRegistrationFieldMask();
        }

        public bool IsEmpty => !(ShouldSerializeRegistrationMode() || ShouldSerializeAutomaticApproval() || ShouldSerializeFieldMask() || ShouldSerializeConvertProvinceAbbreviation());

        public bool ShouldSerializeAutomaticApproval() => AutomaticApproval;
        public bool ShouldSerializeRegistrationMode() => RegistrationMode != UserRegistrationMode.AllowSelfRegistrationOnLogin;
        public bool ShouldSerializeConvertProvinceAbbreviation() => !ConvertProvinceAbbreviation;
        public bool ShouldSerializeFieldMask() => !FieldMask.IsEmpty;
        public bool ShouldSerializeIsEmpty() => false;

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (FieldMask == null)
                FieldMask = new UserRegistrationFieldMask();
        }

        public bool IsEqual(UserRegistration other)
        {
            return RegistrationMode == other.RegistrationMode
                && AutomaticApproval == other.AutomaticApproval
                && ConvertProvinceAbbreviation == other.ConvertProvinceAbbreviation
                && FieldMask.IsEqual(other.FieldMask);
        }
    }
}