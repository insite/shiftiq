using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class AddSurveyCondition : Command
    {
        public AddSurveyCondition(Guid form, Guid maskingItem, Guid[] maskedQuestions)
        {
            AggregateIdentifier = form;
            MaskingItem = maskingItem;
            MaskedQuestions = maskedQuestions;
        }

        public Guid MaskingItem { get; }
        public Guid[] MaskedQuestions { get; }
    }
}