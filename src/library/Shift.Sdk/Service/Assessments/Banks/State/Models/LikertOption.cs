using System;

using Newtonsoft.Json;

namespace InSite.Domain.Banks
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class LikertOption
    {
        [JsonProperty]
        public Guid RowIdentifier { get; private set; }

        [JsonProperty]
        public Guid ColumnIdentifier { get; private set; }

        [JsonProperty]
        public int Number { get; set; }

        [JsonProperty]
        public decimal Points { get; set; }

        public virtual LikertRow Row { get; }

        public virtual LikertColumn Column { get; }

        public bool HasPoints => Points != 0.0m;

        public LikertOption()
        {

        }

        protected LikertOption(Guid row, Guid column) : this()
        {
            RowIdentifier = row;
            ColumnIdentifier = column;
        }

        public void CopyTo(LikertOption option)
        {
            option.Points = Points;
            option.Number = Number;
        }

        public bool ShouldSerializePoints()
        {
            return Points != 0;
        }

        public bool IsEqual(LikertOption other, bool compareIdentifiers = true)
        {
            return (!compareIdentifiers || this.RowIdentifier == other.RowIdentifier && this.ColumnIdentifier == other.ColumnIdentifier)
                && this.Points == other.Points
                && this.Number == other.Number;
        }

        public LikertOption Clone()
        {
            var clone = new LikertOption(RowIdentifier, ColumnIdentifier);

            CopyTo(clone);

            return clone;
        }
    }
}
