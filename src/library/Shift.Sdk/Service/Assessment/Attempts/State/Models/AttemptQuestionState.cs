using System;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Attempts
{
    public class AttemptQuestionState
    {
        [JsonProperty]
        public Guid QuestionIdentifier { get; private set; }

        [JsonProperty]
        public int QuestionIndex { get; private set; }

        [JsonProperty]
        public int? SectionIndex { get; private set; }

        [JsonProperty]
        public Guid[] SubQuestions { get; private set; }

        [JsonProperty]
        public AttemptQuestionRubric Rubric { get; private set; }

        [JsonConstructor]
        private AttemptQuestionState()
        {

        }

        public AttemptQuestionState(AttemptStarted2.QuestionHandle handle, int index)
        {
            QuestionIdentifier = handle.Question;
            QuestionIndex = index;
            SectionIndex = handle.Section;
            SubQuestions = handle.LikertRows.EmptyIfNull();
        }

        public AttemptQuestionState(AttemptQuestion question, int index)
        {
            QuestionIdentifier = question.Identifier;
            QuestionIndex = index;
            SectionIndex = question.Section;
            SubQuestions = new Guid[0];

            if (question is AttemptQuestionLikert likert)
                SubQuestions = likert.Questions.Select(x => x.Identifier).ToArray();
            else if (question is AttemptQuestionComposed composed)
                InitRubric(composed.Rubric);
        }

        public void InitRubric(AttemptQuestionRubric rubric)
        {
            if (Rubric == null)
                Rubric = rubric.Clone();
        }
    }
}
