using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyFormContentChanged : Change
    {
        public SurveyFormContentChanged(ContentContainer content)
        {
            Content = content;
        }

        public ContentContainer Content { get; }
    }
}