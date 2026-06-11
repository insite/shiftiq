using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class AddSurveyOptionItem : Command
    {
        public AddSurveyOptionItem(Guid form, Guid list, Guid item)
        {
            AggregateIdentifier = form;
            List = list;
            Item = item;
        }

        public Guid List { get; }
        public Guid Item { get; }
    }
}