using System;

namespace InSite.Domain.Surveys.Forms
{
    public class ResponseAnalysisCorrelationItem
    {
        public Guid? XSurveyOptionIdentifier { get; set; }
        public Guid? YSurveyOptionIdentifier { get; set; }
        public string XSurveyOptionTitle { get; set; }
        public string YSurveyOptionTitle { get; set; }
        public int YTotalValue { get; set; }
        public int XTotalValue { get; set; }
        public int YValue { get; set; }
        public int XValue { get; set; }
        public int CrossValue { get; set; }
    }
}
