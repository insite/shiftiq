using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class AttributeSurveyQuestion : Command
    {
        public AttributeSurveyQuestion(Guid form, Guid question, string attribute)
        {
            AggregateIdentifier = form;
            Question = question;
            Attribute = attribute;
        }

        public Guid Question { get; set; }
        public string Attribute { get; set; }
    }
}