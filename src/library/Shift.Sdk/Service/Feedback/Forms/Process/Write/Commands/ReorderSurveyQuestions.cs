using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class ReorderSurveyQuestions : Command
    {
        /// <summary>
        /// Dictionary Key   = new sequence value
        /// Dictionary Value = object to which the new sequence value is assigned
        /// </summary>
        public Dictionary<Guid, int> Sequences { get; }

        public ReorderSurveyQuestions(Guid form, Dictionary<Guid, int> sequences)
        {
            AggregateIdentifier = form;
            Sequences = sequences;
        }
    }
}