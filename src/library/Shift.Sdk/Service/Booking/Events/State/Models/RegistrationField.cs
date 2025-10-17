using System;

using Newtonsoft.Json;

namespace InSite.Domain.Events
{
    [Serializable]
    public class RegistrationField
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public RegistrationFieldName FieldName { get; set; }

        public bool IsRequired { get; set; }
        public bool IsVisible { get; set; }
        public bool IsEditable { get; set; }

        public RegistrationField Clone()
        {
            return (RegistrationField)MemberwiseClone();
        }
    }
}
