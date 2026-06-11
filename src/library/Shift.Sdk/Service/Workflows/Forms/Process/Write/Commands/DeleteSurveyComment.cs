using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class DeleteSurveyComment : Command
    {
        public Guid Comment { get; set; }

        public DeleteSurveyComment(Guid form, Guid comment)
        {
            AggregateIdentifier = form;
            Comment = comment;
        }
    }
}
