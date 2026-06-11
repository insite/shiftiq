using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class DeleteSurveyOptionItem : Command
    {
        public DeleteSurveyOptionItem(Guid form, Guid item)
        {
            AggregateIdentifier = form;
            Item = item;
        }

        public Guid Item { get; }
    }
}