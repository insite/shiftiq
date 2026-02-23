using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyFormMessagesChanged : Change
    {
        public SurveyFormMessagesChanged(SurveyMessage[] messages)
        {
            Messages = messages;
        }

        public SurveyMessage[] Messages { get; }
    }
}