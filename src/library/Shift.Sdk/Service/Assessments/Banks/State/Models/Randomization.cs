using System;

using Shift.Common;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// Represents the randomization settings for the items in a list. If randomization is enabled, it may be required
    /// to randomize only the first N items in the list.
    /// </summary>
    [Serializable]
    public class Randomization
    {
        /// <summary>
        /// Indicates randomization is enabled or disabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// If Enabled = True then the Count indicates the number of items in the list to randomize. A value of zero 
        /// is used to specify that all items should be randomized. If Count is greater than zero and less than the
        /// number of items in the list then the first N items are randomized, and items N+1 to List.Count remain in 
        /// their existing sequence.
        /// </summary>
        public int Count { get; set; }

        #region Methods (comparison)

        public bool Equals(Randomization other)
        {
            return this.Enabled == other.Enabled
                && (!this.Enabled || this.Count == other.Count);
        }

        #endregion

        #region Methods (helpers)

        public void Copy(Randomization source)
        {
            source.ShallowCopyTo(this);
        }

        public Randomization Clone()
        {
            var clone = new Randomization();
            clone.Copy(this);
            return clone;
        }

        #endregion
    }
}
