using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Banks
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class LikertColumn
    {
        [JsonProperty]
        public Guid Identifier { get; private set; }

        [JsonProperty]
        public ContentTitle Content { get; set; }

        public string Letter => Calculator.ToBase26(Index + 1);

        public int Index => Matrix.GetIndex(this);

        public int OptionCount => Matrix.RowCount;

        public virtual LikertMatrix Matrix { get; }

        public LikertColumn()
        {
            Content = new ContentTitle();
        }

        protected LikertColumn(Guid id) : this()
        {
            Identifier = id;
        }

        public void Remove() => Matrix.RemoveColumn(Identifier);

        public IEnumerable<LikertOption> GetOptions() => Matrix.Rows.Select(row => Matrix.GetOption(row, this));

        public LikertOption GetOption(int index) => Matrix.GetOption(Matrix.GetRow(index), this);

        public LikertOption GetOption(Guid row) => Matrix.GetOption(row, Identifier);

        public LikertOption GetOption(LikertRow row) => Matrix.GetOption(row, this);

        public void CopyTo(LikertColumn column)
        {
            column.Content = Content.Clone();
        }

        public bool ShouldSerializeContent()
        {
            return Content != null && !Content.IsEmpty;
        }

        public bool IsEqual(LikertColumn other, bool compareIdentifiers = true)
        {
            return (!compareIdentifiers || this.Identifier == other.Identifier)
                && this.Content.IsEqual(other.Content);
        }
    }
}
