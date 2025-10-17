using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Banks
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class HotspotOption
    {
        [JsonProperty]
        public Guid Identifier { get; private set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Number { get; set; }

        [JsonProperty]
        public ContentTitle Content { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public decimal Points { get; set; }

        [JsonProperty]
        public HotspotShape Shape { get; private set; }

        public string Letter => Calculator.ToBase26(Index + 1);

        public int Index => _container.GetOptionIndex(Identifier);

        [NonSerialized]
        private Hotspot _container;

        [JsonConstructor]
        private HotspotOption()
        {
            Content = new ContentTitle();
        }

        public HotspotOption(Guid id, HotspotShape shape)
            : this()
        {
            Identifier = id;
            Shape = shape.Clone();
        }

        internal void SetContainer(Hotspot container)
        {
            if (_container != null)
                throw ApplicationError.Create("Hotspot is already assigned to this option");

            _container = container;
        }

        public void CopyTo(HotspotOption other)
        {
            other.Number = this.Number;
            other.Content = this.Content == null ? new ContentTitle() : this.Content.Clone();
            other.Points = this.Points;
            other.Shape = this.Shape.Clone();
        }

        public HotspotOption Clone()
        {
            var clone = new HotspotOption();
            clone.Identifier = Identifier;

            this.CopyTo(clone);

            return clone;
        }

        public bool IsEqual(HotspotOption other, bool compareIdentifiers = true)
        {
            return (!compareIdentifiers || this.Identifier == other.Identifier)
                && this.Number == other.Number
                && this.Points == other.Points
                && this.Shape.IsEqual(other.Shape)
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
