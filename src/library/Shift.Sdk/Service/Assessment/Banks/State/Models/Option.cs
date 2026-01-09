using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// Represents an answer option on an exam question. In the special case of a Compsition question, it represents an
    /// item in the question's scoring rubric.
    /// </summary>
    [Serializable]
    public class Option : IQuestionAnswer
    {
        /// <summary>
        /// The question that contains the option.
        /// </summary>
        [JsonIgnore]
        public Question Question { get; set; }

        /// <summary>
        /// The standard associated with selecting this option when answer the question. 
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Guid Standard { get; set; }

        /// <summary>
        /// The unique number assigned to this option within the bank that contains it. No two options in a given bank 
        /// are allowed to have the same option number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// The ordinal position of this option in the question that contains it.
        /// </summary>
        [JsonIgnore]
        public int Sequence => 1 + Question.Options.IndexOf(this);

        /// <summary>
        /// The cut-score for this option.
        /// </summary>
        public decimal? CutScore { get; set; }

        /// <summary>
        /// The number of points awarded for selecting this option on an exam question. In the special case of a 
        /// Composed question, the maximum number of points awarded for this item in the scoring rubric.
        /// </summary>
        public decimal Points { get; set; }

        /// <summary>
        /// Returns an alphabetic equivalent for the integer sequence (e.g. A = 1, B = 2, C = 3).
        /// </summary>
        [JsonIgnore]
        public string Letter => Calculator.ToBase26(Sequence);

        /// <summary>
        /// Returns true if number of points for this option is greater than zero.
        /// </summary>
        [JsonIgnore]
        public bool HasPoints => Points != 0.0m;

        /// <summary>
        /// Returns true if the option represents a true (or correct) statement; returns false if the option represents
        /// a false (or incorrect) statement; returns null if the option does not represent a Boolean statement. If the
        /// option is true then points are awarded for selecting it. If the option is false then points are awarded for
        /// not selecting it.
        /// </summary>
        public bool? IsTrue { get; set; }

        /// <summary>
        /// Options need support for multilingual titles.
        /// </summary>
        public ContentTitle Content { get; set; }

        /// <summary>
        /// Constructs an empty option.
        /// </summary>
        public Option()
        {
            Content = new ContentTitle();
        }

        #region Methods (serialization)

        public bool ShouldSerializePoints()
        {
            return HasPoints;
        }

        #endregion

        #region Methods (comparing)

        public bool Equals(Option other)
        {
            return other != null
                && this.Content.IsEqual(other.Content)
                && this.Points == other.Points
                && this.CutScore == other.CutScore
                && this.IsTrue == other.IsTrue
                && this.Standard == other.Standard;
        }

        #endregion

        #region Methods (helpers)

        public void Copy(Option source)
        {
            source.ShallowCopyTo(this);

            Content = source.Content.Clone();
        }

        public Option Clone()
        {
            var clone = new Option();

            clone.Copy(this);

            return clone;
        }

        #endregion
    }
}
