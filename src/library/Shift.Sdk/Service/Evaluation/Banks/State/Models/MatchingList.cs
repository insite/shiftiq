using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;
namespace InSite.Domain.Banks
{
    /// <summary>
    /// A matching list is an array of matching pairs. It is use only in exam questions where Type = Matching.
    /// </summary>
    [Serializable]
    public class MatchingList
    {
        /// <summary>
        /// The list of matching left/right pairs in the array.
        /// </summary>
        public List<MatchingPair> Pairs { get; set; }

        /// <summary>
        /// Additional (and optional) answers, all of which are incorrect. Students see a randomized list that 
        /// includes these distractors with the correct answers in the Matching right-side fields.
        /// </summary>
        public List<ContentTitle> Distractors { get; set; }

        /// <summary>
        /// If there are no pairs then the list is empty.
        /// </summary>
        public bool IsEmpty => Pairs.Count == 0;

        /// <summary>
        /// Constructs an empty list.
        /// </summary>
        public MatchingList()
        {
            Pairs = new List<MatchingPair>();
            Distractors = new List<ContentTitle>();
        }

        #region Methods (serialization)

        public bool ShouldSerializeDistractors()
        {
            return Distractors.Count > 0;
        }

        public bool ShouldSerializePairs()
        {
            return Pairs.Count > 0;
        }

        public bool ShouldSerializePoints()
        {
            return false;
        }

        #endregion

        #region Methods (helpers)

        public bool Equals(MatchingList other)
        {
            return this.Pairs.Count == other.Pairs.Count && (this.Pairs.Count == 0 || this.Pairs.Zip(other.Pairs, (a, b) => a.Equals(b)).All(x => x))
                && this.Distractors.Count == other.Distractors.Count && (this.Distractors.Count == 0 || this.Distractors.Zip(other.Distractors, (a, b) => a.IsEqual(b)).All(x => x));
        }

        public MatchingList Clone()
        {
            return new MatchingList
            {
                Pairs = Pairs.Select(x => x.Clone()).ToList(),
                Distractors = Distractors.Select(x => x.Clone()).ToList(),
            };
        }

        #endregion
    }
}
