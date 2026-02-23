using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// Represents a question set in a question bank. A set is a group of related question items.
    /// </summary>
    [Serializable]
    public class Set
    {
        [JsonIgnore]
        public BankState Bank { get; set; }

        public Guid Identifier { get; set; }

        /// <summary>
        /// Uniquely identifies the standard (e.g. general area of competency) evaluated by the questions in the set.
        /// If the set does not evaluate any standard then Guid.Empty is assumed as a sentinel value.
        /// </summary>
        public Guid Standard { get; set; }

        /// <summary>
        /// The internal name used to uniquely identify this set for filing purposes.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns an alphabetic equivalent for the integer sequence (e.g. A = 1, B = 2, C = 3).
        /// </summary>
        [JsonIgnore]
        public string Letter => Calculator.ToBase26(Sequence);

        /// <summary>
        /// The ordinal position of this set in the bank that contains it.
        /// </summary>
        [JsonIgnore]
        public int Sequence => 1 + Bank.Sets.IndexOf(this);

        /// <summary>
        /// In the special-case of a Scenario question set, this is the cut-score for this question set. This property
        /// does not apply to sets where Type = Pool.
        /// </summary>
        public decimal? CutScore { get; set; }

        /// <summary>
        /// In the special-case of a Scenario question set, this is the maximum number of points that can be awarded 
        /// for answers to the question items in the set. This property does not apply to sets where Type = Pool.
        /// </summary>
        public decimal? Points { get; set; }

        /// <summary>
        /// The randomization settings for the questions in the set. Any section that displays question items in this 
        /// set is expected to use the same randomization settings for display of the questions it contains.
        /// </summary>
        public Randomization Randomization { get; set; }

        /// <summary>
        /// The questions contained by the set.
        /// </summary>
        public List<Question> Questions { get; set; }

        /// <summary>
        /// The sieves that reference the set.
        /// </summary>
        [JsonIgnore]
        public List<Criterion> Criteria { get; set; }

        /// <summary>
        /// Constructs an empty set.
        /// </summary>
        public Set()
        {
            Questions = new List<Question>();
            Randomization = new Randomization();
            Criteria = new List<Criterion>();
        }

        public Set Clone()
        {
            var clone = new Set();

            this.ShallowCopyTo(clone);

            clone.Randomization = Randomization.Clone();
            clone.Questions = Questions.EmptyIfNull().Select(x => x.Clone(true)).ToList();

            clone.RestoreReferences();

            return clone;
        }

        #region Methods (serialization)

        public bool ShouldSerializeRandomization()
        {
            return Randomization.Enabled;
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            RestoreReferences();
        }

        #endregion

        #region Methods (overriden)

        public override string ToString()
        {
            return $"Set {Letter} - {Name}";
        }

        #endregion

        #region Methods (helpers)

        public IEnumerable<Question> EnumerateAllQuestions() =>
            Questions.SelectMany(x => x.EnumerateAllVersions());

        internal void RestoreReferences()
        {
            foreach (var question in EnumerateAllQuestions())
                question.Set = this;
        }

        #endregion
    }
}
