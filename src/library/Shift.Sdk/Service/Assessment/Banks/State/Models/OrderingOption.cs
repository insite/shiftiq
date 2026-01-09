using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Banks
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class OrderingOption
    {
        [JsonProperty]
        public Guid Identifier { get; private set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Number { get; set; }

        [JsonProperty]
        public ContentTitle Content { get; private set; }

        private OrderingOption()
        {
            Content = new ContentTitle();
        }

        public OrderingOption(Guid id) 
            : this()
        {
            Identifier = id;
        }

        public void CopyTo(OrderingOption other)
        {
            other.Number = this.Number;
            other.Content = this.Content.Clone();
        }

        public OrderingOption Clone()
        {
            var clone = new OrderingOption();
            clone.Identifier = Identifier;

            this.CopyTo(clone);

            return clone;
        }

        public bool IsEqual(OrderingOption other, bool compareIdentifiers = true)
        {
            return (!compareIdentifiers || this.Identifier == other.Identifier)
                && this.Number == other.Number
                && this.Content.IsEqual(other.Content);
        }

        #region Methods (serialization)

        public bool ShouldSerializeContent()
        {
            return Content?.IsEmpty == false;
        }

        #endregion
    }
}
