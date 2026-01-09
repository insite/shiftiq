using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Attempts.Write
{
    public class ScoreComposedQuestion : Command
    {
        public Guid Question { get; }
        public Dictionary<Guid, decimal> RubricRatingPoints { get; }

        public ScoreComposedQuestion(Guid aggregate, Guid question, Dictionary<Guid, decimal> rubricRatingPoints)
        {
            AggregateIdentifier = aggregate;
            Question = question;
            RubricRatingPoints = rubricRatingPoints;
        }
    }
}
