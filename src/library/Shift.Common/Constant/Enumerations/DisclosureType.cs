namespace Shift.Constant
{
    public enum DisclosureType
    {
        /// <summary>
        /// Neither final scores nor marked questions are revealed to candidates after they complete their exam 
        /// submissions.
        /// </summary>
        None,

        /// <summary>
        /// Only the total/final score is revealed to candidates, without individually marked questions.
        /// </summary>
        Score,

        /// <summary>
        /// Only the marked questions are revealed to candidates, without total/final score.
        /// </summary>
        Answers,

        /// <summary>
        /// Candidates see the final score and the marked questions immediately after they complete their exam
        /// submissions, and upon subsequent review of their exam submissions.
        /// </summary>
        Full
    }
}