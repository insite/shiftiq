using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class DeleteSurveyCondition : Command
    {
        public DeleteSurveyCondition(Guid form, Guid maskingItem, Guid[] maskedQuestions)
        {
            AggregateIdentifier = form;
            MaskingItem = maskingItem;
            MaskedQuestions = maskedQuestions;
        }

        public Guid MaskingItem { get; }
        public Guid[] MaskedQuestions { get; }
    }
}