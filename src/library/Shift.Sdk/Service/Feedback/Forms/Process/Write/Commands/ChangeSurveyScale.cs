using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Surveys.Forms;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyScale : Command
    {
        public ChangeSurveyScale(Guid form, Guid question, SurveyScale scale)
        {
            AggregateIdentifier = form;
            Question = question;
            Scale = scale;
        }

        public Guid Question { get; set; }
        public SurveyScale Scale { get; }
    }
}