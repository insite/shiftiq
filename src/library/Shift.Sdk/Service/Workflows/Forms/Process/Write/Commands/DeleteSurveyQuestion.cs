using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class DeleteSurveyQuestion : Command
    {
        public DeleteSurveyQuestion(Guid form, Guid question)
        {
            AggregateIdentifier = form;
            Question = question;
        }

        public Guid Question { get; set; }
    }
}