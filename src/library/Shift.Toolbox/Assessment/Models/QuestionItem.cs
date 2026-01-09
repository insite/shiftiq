
using System.Collections.Generic;

namespace Shift.Toolbox.Assessments.Models
{
    public class QuestionItem
    {
        public string SectionTitle { get; set; }
        public string SectionSummary { get; set; }
        public int Number { get; set; }
        public int Sequence { get; set; }
        public decimal Points { get; set; }
        public decimal? AnswerPoints { get; set; }
        public string AnswerText { get; set; }
        public string AnswerFileUrl { get; set; }
        public int AnswerAttemptLimit { get; set; }
        public int AnswerRequestAttempt { get; set; }
        public string TagsAndLabels { get; set; }
        public string Text { get; set; }

        public bool IsListBox { get; set; }
        public string ListGroupClass { get; set; }

        public bool IsComposedEssay { get; set; }
        public bool IsComposedVoice { get; set; }
        public bool IsBooleanTable { get; set; }
        public bool IsMatching { get; set; }
        public bool IsLikert { get; set; }
        public bool IsHotspot { get; set; }
        public bool IsOrdering { get; set; }

        public bool HasRationale { get; set; }
        public bool HasRationaleForCorrect { get; set; }
        public bool HasRationaleForIncorrect { get; set; }
        public bool HasFeedbackPoints { get; set; }
        public bool HasAdditionalPanel => HasRationale || HasRationaleForCorrect || HasRationaleForIncorrect || HasFeedbackPoints;

        public string RationaleText { get; set; }
        public string RationaleTextOnCorrectAnswer { get; set; }
        public string RationaleTextOnIncorrectAnswer { get; set; }

        public string HotspotImageData { get; set; }
        public string HotspotShapes { get; set; }
        public string HotspotPins { get; set; }

        public string OrderingTopLabel { get; set; }
        public string OrderingBottomLabel { get; set; }

        public List<OptionHeaderItem> OptionHeaders { get; set; }
        public List<OptionItem> Options { get; set; }
        public List<MatchItem> Matches { get; set; }
        public List<QuestionItem> LikertQuestions { get; set; }
    }
}
