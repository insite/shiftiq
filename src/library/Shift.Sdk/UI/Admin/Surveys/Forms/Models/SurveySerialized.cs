using System;
using System.Collections.Generic;

using Shift.Constant;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class SurveySerialized
    {
        public SurveyFormStatus Status { get; set; }

        public string Name { get; set; }
        public string Hook { get; set; }
        public string Language { get; set; }
        public string[] LanguageTranslations { get; set; }

        public bool EnableUserConfidentiality { get; set; }
        public UserFeedbackType UserFeedback { get; set; }
        public bool RequireUserAuthentication { get; set; }
        public bool RequireUserIdentification { get; set; }
        public bool DisplaySummaryChart { get; set; }

        public int? ResponseLimitPerUser { get; set; }
        public int? DurationMinutes { get; set; }


        public List<SurveyContentSerialized> Content { get; set; }
        public List<QuestionSerialized> Questions { get; set; }
    }
}