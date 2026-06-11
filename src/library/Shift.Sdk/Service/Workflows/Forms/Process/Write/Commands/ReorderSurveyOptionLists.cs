using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class ReorderSurveyOptionLists : Command
    {
        /// <summary>
        /// Container for the reordered objects.
        /// </summary>
        public Guid Question { get; set; }

        /// <summary>
        /// Dictionary Key   = new sequence value
        /// Dictionary Value = object to which the new sequence value is assigned
        /// </summary>
        public Dictionary<Guid, int> Sequences { get; }

        public ReorderSurveyOptionLists(Guid form, Guid question, Dictionary<Guid, int> sequences)
        {
            AggregateIdentifier = form;
            Question = question;
            Sequences = sequences;
        }
    }
}