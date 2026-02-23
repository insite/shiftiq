using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class AddSurveyOptionList : Command
    {
        public AddSurveyOptionList(Guid form, Guid question, Guid list)
        {
            AggregateIdentifier = form;
            Question = question;
            List = list;
        }

        public Guid Question { get; set; }
        public Guid List { get; }
    }
}