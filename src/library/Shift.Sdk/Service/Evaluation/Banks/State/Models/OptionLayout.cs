using System;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// Represents the layout configuration for a question containing options that are displayed to a student/candidate
    /// in a multi-column table.
    /// </summary>
    [Serializable]
    public class OptionLayout
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public OptionLayoutType Type { get; set; }

        public OptionColumn[] Columns { get; set; }

        #region Methods (comparison)

        public bool Equals(OptionLayout other)
        {
            var thisColumns = this.Columns ?? new OptionColumn[0];
            var otherColumns = other.Columns ?? new OptionColumn[0];

            return this.Type == other.Type
                && thisColumns.Length == otherColumns.Length
                && (thisColumns.Length == 0
                    || thisColumns.Zip(otherColumns, (i1, i2) => i1.Equals(i2)).All(x => x));
        }

        #endregion

        #region Methods (helpers)

        public void Copy(OptionLayout source)
        {
            source.ShallowCopyTo(this);

            Columns = source.Columns?.Select(x => x.Clone()).ToArray();
        }

        public OptionLayout Clone()
        {
            var clone = new OptionLayout();
            clone.Copy(this);
            return clone;
        }

        #endregion
    }
}
