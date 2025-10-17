using System;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ListResultItemModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid FormIdentifier { get; set; }

        [JsonProperty(PropertyName = "asset")]
        public int FormAsset { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int FormAssetVersion { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string FormName { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string FormTitle { get; set; }

        public Guid BankId { get; set; }

        [JsonProperty(PropertyName = "bankAsset")]
        public int? BankAsset { get; set; }

        [JsonProperty(PropertyName = "bankTitle")]
        public string BankTitle { get; set; }
    }
}