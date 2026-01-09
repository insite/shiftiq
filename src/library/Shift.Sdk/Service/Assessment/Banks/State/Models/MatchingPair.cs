using System;

using Shift.Common;
namespace InSite.Domain.Banks
{
    /// <summary>
    /// A matching pair is an item in a matching list. It is use only in exam questions where Type = Matching.
    /// </summary>
    [Serializable]
    public class MatchingPair : IQuestionAnswer
    {
        #region Propeties

        /// <summary>
        /// The item on the left in a pair.
        /// </summary>
        public ContentTitle Left { get; set; }

        /// <summary>
        /// The matching (i.e. correct) item on the right in a pair.
        /// </summary>
        public ContentTitle Right { get; set; }

        /// <summary>
        /// The cut-score for this pair.
        /// </summary>
        public decimal? CutScore { get; set; }

        /// <summary>
        /// The number of points awarded for a correct selection.
        /// </summary>
        public decimal Points { get; set; }

        #endregion

        #region Construction

        public MatchingPair()
        {
            Left = new ContentTitle();
            Right = new ContentTitle();
        }

        #endregion

        #region Methods (helpers)

        public bool Equals(MatchingPair other)
        {
            return this.Left.IsEqual(other.Left)
                && this.Right.IsEqual(other.Right)
                && this.CutScore == other.CutScore
                && this.Points == other.Points;
        }

        public MatchingPair Clone()
        {
            return new MatchingPair
            {
                Left = Left.Clone(),
                Right = Right.Clone(),
                CutScore = CutScore,
                Points = Points,
            };
        }

        #endregion

        #region IQuestionAnswer

        bool? IQuestionAnswer.IsTrue => true;

        #endregion
    }
}
