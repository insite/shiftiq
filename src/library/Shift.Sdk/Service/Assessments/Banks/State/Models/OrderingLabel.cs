using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Banks
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class OrderingLabel
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Show { get; set; }

        [JsonProperty]
        public ContentTitle TopContent { get; private set; } = new ContentTitle();

        [JsonProperty]
        public ContentTitle BottomContent { get; private set; } = new ContentTitle();

        public bool IsEmpty => Show == default && TopContent.IsEmpty && BottomContent.IsEmpty;

        public void CopyTo(OrderingLabel other)
        {
            other.Show = this.Show;
            other.TopContent = this.TopContent.Clone();
            other.BottomContent = this.BottomContent.Clone();
        }

        public OrderingLabel Clone()
        {
            var clone = new OrderingLabel();

            this.CopyTo(clone);

            return clone;
        }

        public bool IsEqual(OrderingLabel other)
        {
            return this.Show == other.Show
                && this.TopContent.IsEqual(other.TopContent)
                && this.BottomContent.IsEqual(other.BottomContent);
        }

        #region Methods (serialization)

        public bool ShouldSerializeTopContent()
        {
            return TopContent?.IsEmpty == false;
        }

        public bool ShouldSerializeBottomContent()
        {
            return BottomContent?.IsEmpty == false;
        }

        #endregion
    }
}
