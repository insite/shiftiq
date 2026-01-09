using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Attempts
{
    public class ComposedQuestionScored : Change
    {
        public Guid Question { get; }
        public Dictionary<Guid, decimal> RubricRatingPoints { get; }

        public ComposedQuestionScored(Guid question, Dictionary<Guid, decimal> rubricRatingPoints)
        {
            Question = question;
            RubricRatingPoints = rubricRatingPoints;
        }
    }
}
