using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Banks
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class LikertRow
    {
        [JsonProperty]
        public Guid Identifier { get; private set; }

        [JsonProperty]
        public Guid Standard { get; set; }

        [JsonProperty]
        public Guid[] SubStandards { get; set; }

        [JsonProperty]
        public ContentTitle Content { get; set; }

        /// <summary>
        /// Key is the FormId
        /// Value is the GradeItemId
        /// </summary>
        [JsonProperty]
        public Dictionary<Guid, Guid> GradeItems { get; set; }

        public string Letter => Calculator.ToBase26(Index + 1);

        public int Index => Matrix.GetIndex(this);

        public int OptionCount => Matrix.ColumnCount;

        public decimal? Points => OptionCount == 0 ? (decimal?)null : GetOptions().Max(x => x.Points);

        public virtual LikertMatrix Matrix { get; }

        public LikertRow()
        {
            Content = new ContentTitle();
            GradeItems = new Dictionary<Guid, Guid>();
        }

        protected LikertRow(Guid id) : this()
        {
            Identifier = id;
        }

        public void Remove() => Matrix.RemoveRow(Identifier);

        public IEnumerable<LikertOption> GetOptions() => Matrix.Columns.Select(column => Matrix.GetOption(this, column));

        public LikertOption GetOption(int index) => Matrix.GetOption(this, Matrix.GetColumn(index));

        public LikertOption GetOption(Guid column) => Matrix.GetOption(Identifier, column);

        public LikertOption GetOption(LikertColumn column) => Matrix.GetOption(this, column);

        public void CopyTo(LikertRow row)
        {
            row.Standard = this.Standard;
            row.SubStandards = this.SubStandards?.ToArray();
            row.Content = this.Content.Clone();
            row.GradeItems = new Dictionary<Guid, Guid>(this.GradeItems);
        }

        public bool ShouldSerializeStandard()
        {
            return Standard != Guid.Empty;
        }

        public bool ShouldSerializeSubStandards()
        {
            return SubStandards.IsNotEmpty();
        }

        public bool ShouldSerializeContent()
        {
            return Content != null && !Content.IsEmpty;
        }

        public bool IsEqual(LikertRow other, bool compareIdentifiers = true)
        {
            return (!compareIdentifiers || this.Identifier == other.Identifier)
                && this.Standard == other.Standard
                && this.SubStandards.EmptyIfNull().OrderBy(x => x).SequenceEqual(other.SubStandards.EmptyIfNull().OrderBy(x => x))
                && this.Content.IsEqual(other.Content);
        }
    }
}
